using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using Container = Malama.Models.Container;
using Microsoft.AspNetCore.Identity;
using Azure;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ContainerMonitoringService : IContainerMonitoringService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;


        public ContainerMonitoringService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<IEnumerable<ContainerType>> GetAllContainerTypesAsync()
        {
            return await _unitOfWork.ContainerType.GetAllAsync();
        }


        public async Task<List<Container>> GetContainersByEventIdAsync(string eventId)
        {
            return await _unitOfWork.Container
                .GetWithInclude(x => x.EventId == eventId, x => x.ContainerType)
                .ToListAsync(); // <-- Materialize query
        }

        public async Task<List<Container>> GetOnlyContainersByEventIdAsync(string eventId)
        {
            return await _unitOfWork.Container.FindForSearching(f => f.EventId == eventId).ToListAsync();
        }


        public async Task<Container?> GetContainerByIdAsync(long id)
        {
            return _unitOfWork.Container
                .GetWithInclude(x => x.Id == id, x => x.ContainerType).FirstOrDefault();
        }

        public async Task<List<ContainerTemperatureReading>> GetReadingsForContainer(long containerId)
        {
            try
            {
                var readings = await _unitOfWork.ContainerTemperatureReading
                    .FindAllAsync(r => r.ContainerId == containerId);

                return readings
                    .OrderByDescending(r => r.ReadingTimeUtc)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Optionally log: _logger.LogError(ex, "Error fetching readings for container {ContainerId}", containerId);
                throw new Exception($"An error occurred while fetching temperature readings for container ID {containerId}.", ex);
            }
        }

        public async Task<ResponseDto> AddContainerAsync(CreateContainerDto dto, string addedBy)
        {
            try
            {
                var containerType = await _unitOfWork.ContainerType.FindAsync(c => c.Id == dto.ContainerTypeId);
                if (containerType == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid container type selected."
                    };
                }

                var existingContainer = await _unitOfWork.Container.FindAsync(c =>
            c.ContainerName.ToLower() == dto.ContainerName.ToLower().Trim() &&
            c.EventId == dto.EventId
        );

                if (existingContainer != null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = $"A container with the name '{dto.ContainerName}' already exists for this event."
                    };
                }

                bool isOutOfRange = dto.InitialTemperature < containerType.TemperatureFromRange ||
                                    dto.InitialTemperature > containerType.TemperatureToRange;

                if (isOutOfRange && string.IsNullOrWhiteSpace(dto.Comment))
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Comment is required when temperature is out of range."
                    };
                }

                // 3️⃣ Build entity
                var container = new Container
                {
                    EventId = dto.EventId,
                    ContainerName = dto.ContainerName,
                    ContainerTypeId = dto.ContainerTypeId,
                    StartDateTimeUtc = DateTime.Now,
                    InitialTemperature = dto.InitialTemperature,
                    Comment = dto.Comment,
                    AddedBy = addedBy,
                    AddedOn = DateTime.Now,
                    MonitoringIntervalMinutes = 120,
                    EscalationIntervalMinutes = 15,
                    ConsecutiveNormalReadings = isOutOfRange ? 0 : 1,
                    CurrentStatus = isOutOfRange ? "OutOfRange" : "Normal",
                    NextExpectedReadingUtc = DateTime.Now.AddMinutes(isOutOfRange ? 15 : 120)
                };

                // 4️⃣ Save
                await _unitOfWork.Container.AddAsync(container);
                await _unitOfWork.SaveAsync();

                return new ResponseDto
                {
                    Success = true,
                    Message = "Container added successfully.",
                };
            }
            catch (ArgumentException ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while adding the container.",
                    Data = null
                };
            }
        }




        public async Task<ResponseDto> AddReadingAsync(CreateReadingDto dto, string performedBy)
        {
            try
            {
                var container = await _unitOfWork.Container.GetByIdAsync(dto.ContainerId);
                if (container == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid container ID."
                    };
                }

                if (!dto.Temperature.HasValue)
                {
                    // Only update FinalTemp if it actually changed
                    if (container.FinalTemp != dto.IsFinalReading)
                    {
                        container.FinalTemp = dto.IsFinalReading;
                        await _unitOfWork.Container.UpdateAsync(container);
                        await _unitOfWork.SaveAsync();
                    }

                    return new ResponseDto
                    {
                        Success = true,
                        Message = "Final reading updated successfully (no temperature provided)."
                    };
                }

                // Return null because no new reading record should be added

                var containerType = await _unitOfWork.ContainerType.GetByIdAsync(container.ContainerTypeId);
                if (containerType == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid container type."
                    };
                }

                var reading = new ContainerTemperatureReading
                {
                    ContainerId = dto.ContainerId,
                    ReadingTimeUtc = DateTime.Now,
                    Temperature = dto.Temperature.Value,
                    Comment = dto.Comment,
                    AddedBy = performedBy
                };


                bool isOutOfRange = dto.Temperature < container?.ContainerType?.TemperatureFromRange ||
                                    dto.Temperature > container?.ContainerType?.TemperatureToRange;
                reading.IsOutOfRange = isOutOfRange;

                var readings = await _unitOfWork.ContainerTemperatureReading
                    .FindAllAsync(r => r.ContainerId == container.Id);

                var lastReading = readings.OrderByDescending(r => r.ReadingTimeUtc).FirstOrDefault();
                int prevAttempt = lastReading?.AttemptNumber ?? 0;

                if (isOutOfRange)
                {
                    reading.AttemptNumber = prevAttempt + 1;
                    container.CurrentStatus = "OutOfRange";
                    container.ConsecutiveNormalReadings = 0;
                    container.NextExpectedReadingUtc = DateTime.Now.AddMinutes(container.EscalationIntervalMinutes);
                }
                else
                {
                    reading.AttemptNumber = 0;
                    container.ConsecutiveNormalReadings += 1;
                    container.CurrentStatus = "Normal";

                    container.NextExpectedReadingUtc = container.ConsecutiveNormalReadings >= 2
                        ? DateTime.Now.AddMinutes(container.MonitoringIntervalMinutes)
                        : DateTime.Now.AddMinutes(container.EscalationIntervalMinutes);
                }

                container.FinalTemp = dto.IsFinalReading;
                await _unitOfWork.ContainerTemperatureReading.AddAsync(reading);
                await _unitOfWork.SaveAsync();

                return new ResponseDto
                {
                    Success = true,
                    Message = "Reading added successfully.",
                    Data = reading
                };
            }
            catch (ArgumentException ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = $"An unexpected error occurred while saving the reading: {ex.Message}"
                };
            }
        }

        public async Task AcknowledgeNotificationAsync(long notificationId)
        {
            try
            {
                var notification = await _unitOfWork.ContainerNotification
                    .FindAsync(n => n.Id == notificationId);

                if (notification != null && !notification.IsAcknowledged)
                {
                    notification.IsAcknowledged = true;
                    notification.AcknowledgedAt = DateTime.Now;
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your preferred logging approach)
                // Example: _logger.LogError(ex, $"Failed to acknowledge notification {notificationId}");
                throw new ApplicationException($"Error acknowledging notification {notificationId}", ex);
            }
        }


    }
}
