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

        public async Task<Container> AddContainerAsync(CreateContainerDto dto, string addedBy)
        {
            try
            {
                var containerType = await _unitOfWork.ContainerType.FindAsync(c => c.Id == dto.ContainerTypeId)
                    ?? throw new ArgumentException("Invalid container type.");

                var container = new Container
                {
                    EventId = dto.EventId,
                    ContainerName = dto.ContainerName,
                    ContainerTypeId = dto.ContainerTypeId,
                    StartDateTimeUtc = dto.StartDate.Date + dto.StartTime,
                    InitialTemperature = dto.InitialTemperature,
                    CurrentStatus = "Normal",
                    MonitoringIntervalMinutes = 120,
                    EscalationIntervalMinutes = 15,
                    ConsecutiveNormalReadings = 0,
                    AddedBy = addedBy,
                    AddedOn = DateTime.Now
                };

                // Determine initial status and schedule
                bool isOutOfRange = dto.InitialTemperature < containerType.TemperatureFromRange ||
                                    dto.InitialTemperature > containerType.TemperatureToRange;

                container.CurrentStatus = isOutOfRange ? "OutOfRange" : "Normal";
                container.NextExpectedReadingUtc = DateTime.Now.AddMinutes(
                    isOutOfRange ? container.EscalationIntervalMinutes : container.MonitoringIntervalMinutes
                );

                // 1️⃣ Save to DB
                await _unitOfWork.Container.AddAsync(container);
                await _unitOfWork.SaveAsync();

                return container;
            }
            catch (ArgumentException)
            {
                // Re-throw validation-type exceptions (controller handles message)
                throw;
            }
            catch (Exception ex)
            {
                // Optionally log exception before rethrowing
                throw new Exception("Error occurred while adding a new container.", ex);
            }
        }


        public async Task<ContainerTemperatureReading> AddReadingAsync(CreateReadingDto dto, string performedBy)
        {
            try
            {
                //var container = _unitOfWork.Container.GetWithInclude(x => x.Id == dto.ContainerId, x => x.ContainerType).FirstOrDefault();

                //if (container == null || container.ContainerType == null)
                //{
                //    throw new ArgumentException("Invalid container ID.");
                //}

                var container = await _unitOfWork.Container.GetByIdAsync(dto.ContainerId)
                    ?? throw new ArgumentException("Invalid container ID.");

                var containerType = await _unitOfWork.ContainerType.GetByIdAsync(container.ContainerTypeId)
                    ?? throw new ArgumentException("Invalid container type.");

                var reading = new ContainerTemperatureReading
                {
                    ContainerId = dto.ContainerId,
                    ReadingTimeUtc = DateTime.Now,
                    Temperature = dto.Temperature,
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

                await _unitOfWork.ContainerTemperatureReading.AddAsync(reading);
                await _unitOfWork.SaveAsync();

                return reading;
            }
            catch (ArgumentException)
            {
                throw; // Let controller handle validation-style exceptions
            }
            catch (Exception ex)
            {
                // Optionally log this: _logger.LogError(ex, "Error adding container reading for container ID {id}", dto.ContainerId);
                throw new Exception($"An error occurred while saving the reading for container ID {dto.ContainerId}.", ex);
            }
        }

    }
}
