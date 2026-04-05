using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Container = Malama.Models.Container;
using Microsoft.AspNetCore.Identity;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ContainerMonitoringService : IContainerMonitoringService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly ILogger<ContainerMonitoringService> _logger;
        private const string CLASSNAME = "ContainerMonitoringService";


        public ContainerMonitoringService(ILogger<ContainerMonitoringService> logger, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _submissionTokenService = submissionTokenService;
            _logger = logger;
        }


        public IQueryable<ContainerType> GetAllContainerTypes()
        {
            return _unitOfWork.ContainerType.GetAllNoTracking();
        }

        public async Task<List<Container>> GetContainersByEventIdAsync(long eventId)
        {
            const string methodName = "GetContainersByEventIdAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, eventId);

            var containers = await _unitOfWork.Container
                .GetWithIncludeNoTracking(x => x.EventManagementId == eventId, x => x.ContainerType)
                .ToListAsync();

            _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {ContainerCount} containers for EventID: {EventID}",
                CLASSNAME, methodName, containers.Count, eventId);

            return containers;
        }


        public async Task<List<Container>> GetOnlyContainersByEventIdAsync(long eventId)
        {
            const string methodName = "GetOnlyContainersByEventIdAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                CLASSNAME, methodName, eventId);

            var containers = await _unitOfWork.Container.GetAllWithConditionNoTracking(f => f.EventManagementId == eventId).ToListAsync();

            _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {ContainerCount} containers for EventID: {EventID}",
                CLASSNAME, methodName, containers.Count, eventId);

            return containers;
        }


        public async Task<Container?> GetContainerByIdAsync(long id)
        {
            const string methodName = "GetContainerByIdAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ContainerID: {ContainerID}",
                CLASSNAME, methodName, id);

            var container = _unitOfWork.Container
                .GetWithIncludeNoTracking(x => x.Id == id, x => x.ContainerType)
                .FirstOrDefault();

            if (container == null)
            {
                _logger.LogWarning("{ClassName}, {MethodName}, Container not found. ContainerID: {ContainerID}",
                    CLASSNAME, methodName, id);
            }
            else
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Container retrieved successfully. ContainerID: {ContainerID}, ContainerName: {ContainerName}",
                    CLASSNAME, methodName, container.Id, container.ContainerName);
            }

            return container;
        }


        public async Task<List<ContainerTemperatureReading>> GetReadingsForContainer(long containerId)
        {
            const string methodName = "GetReadingsForContainer";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ContainerID: {ContainerID}",
                CLASSNAME, methodName, containerId);

            try
            {
                var readings = _unitOfWork.ContainerTemperatureReading
                    .GetAllWithConditionNoTracking(r => r.ContainerId == containerId);

                var orderedReadings = readings.OrderByDescending(r => r.ReadingTimeUtc).ToList();

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {ReadingCount} readings for ContainerID: {ContainerID}",
                    CLASSNAME, methodName, orderedReadings.Count, containerId);

                return orderedReadings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error fetching readings for ContainerID: {ContainerID}",
                    CLASSNAME, methodName, containerId);
                throw new Exception($"An error occurred while fetching temperature readings for container ID {containerId}.", ex);
            }
        }


        public async Task<ResponseDto> AddContainerAsync(CreateContainerDto dto, string addedBy)
        {
            const string methodName = "AddContainerAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called by User: {UserName}, ContainerName: {ContainerName}, EventID: {EventID}",
                CLASSNAME, methodName, addedBy, dto.ContainerName, dto.EventId);

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(dto.SubmissionToken, addedBy);

                if (!tokenResult.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Submission token invalid. User: {UserName}", CLASSNAME, methodName, addedBy);
                    return tokenResult;
                }

                var containerType = await _unitOfWork.ContainerType.GetFirstOrDefaultWithConditionNoTracking(c => c.Id == dto.ContainerTypeId);
                if (containerType == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid ContainerTypeId: {ContainerTypeId}", CLASSNAME, methodName, dto.ContainerTypeId);
                    return new ResponseDto { Success = false, Message = "Invalid container type selected." };
                }

                var existingContainer = _unitOfWork.Container.GetAllWithConditionNoTracking(c =>
                    c.ContainerName.ToLower() == dto.ContainerName.ToLower().Trim() &&
                    c.EventManagementId == dto.EventId
                );

                if (existingContainer.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Container already exists: {ContainerName}, EventID: {EventID}",
                        CLASSNAME, methodName, dto.ContainerName, dto.EventId);

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
                    _logger.LogWarning("{ClassName}, {MethodName}, Out-of-range temperature without comment. ContainerName: {ContainerName}, EventID: {EventID}",
                        CLASSNAME, methodName, dto.ContainerName, dto.EventId);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Comment is required when temperature is out of range."
                    };
                }

                var container = new Container
                {
                    EventManagementId = dto.EventId,
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

                await _unitOfWork.Container.AddAsync(container);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Container added successfully. ContainerID: {ContainerID}, User: {UserName}",
                    CLASSNAME, methodName, container.Id, addedBy);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Container added successfully."
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, ArgumentException while adding container. ContainerName: {ContainerName}",
                    CLASSNAME, methodName, dto.ContainerName);

                return new ResponseDto { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception while adding container. ContainerName: {ContainerName}, EventID: {EventID}",
                    CLASSNAME, methodName, dto.ContainerName, dto.EventId);

                return new ResponseDto { Success = false, Message = "An unexpected error occurred while adding the container." };
            }
        }


        public async Task<ResponseDto> AddReadingAsync(CreateReadingDto dto, string performedBy)
        {
            const string methodName = "AddReadingAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called by User: {UserName}, ContainerID: {ContainerID}, Temperature: {Temperature}",
                CLASSNAME, methodName, performedBy, dto.ContainerId, dto.Temperature);

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(dto.SubmissionToken, performedBy);
                if (!tokenResult.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Submission token invalid. User: {UserName}, ContainerID: {ContainerID}",
                        CLASSNAME, methodName, performedBy, dto.ContainerId);
                    return tokenResult;
                }

                var container = await _unitOfWork.Container.GetByIdAsync(dto.ContainerId);

                if (container == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid ContainerID: {ContainerID}",
                        CLASSNAME, methodName, dto.ContainerId);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid container ID."
                    };
                }

                if (!dto.Temperature.HasValue)
                {
                    if (container.FinalTemp != dto.IsFinalReading)
                    {
                        container.FinalTemp = dto.IsFinalReading;

                        container.UpdatedBy = performedBy;
                        container.UpdatedOn = DateTime.Now;

                        await _unitOfWork.SaveAsync();
                    }

                    return new ResponseDto
                    {
                        Success = true,
                        Message = "Final reading updated successfully (no temperature provided)."
                    };
                }

                var containerType = await _unitOfWork.ContainerType.GetByIdAsync(container.ContainerTypeId);
                if (containerType == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid ContainerTypeID: {ContainerTypeID}", CLASSNAME, methodName, container.ContainerTypeId);
                    return new ResponseDto { Success = false, Message = "Invalid container type." };
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

                var readings = _unitOfWork.ContainerTemperatureReading.GetAllWithConditionNoTracking(r => r.ContainerId == container.Id);
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

                _logger.LogInformation("{ClassName}, {MethodName}, Reading added successfully. ContainerID: {ContainerID}, User: {UserName}, Temperature: {Temperature}, OutOfRange: {OutOfRange}",
                    CLASSNAME, methodName, dto.ContainerId, performedBy, dto.Temperature, isOutOfRange);

                return new ResponseDto { Success = true, Message = "Reading added successfully.", Data = reading };
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, ArgumentException while adding reading. ContainerID: {ContainerID}", CLASSNAME, methodName, dto.ContainerId);
                return new ResponseDto { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception while adding reading. ContainerID: {ContainerID}", CLASSNAME, methodName, dto.ContainerId);
                return new ResponseDto { Success = false, Message = $"An unexpected error occurred while saving the reading: {ex.Message}" };
            }
        }


        public async Task AcknowledgeNotificationAsync(long notificationId)
        {
            const string methodName = "AcknowledgeNotificationAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with NotificationID: {NotificationID}", CLASSNAME, methodName, notificationId);

            try
            {
                var notification = await _unitOfWork.ContainerNotification.GetFirstOrDefaultWithConditionNoTracking(n => n.Id == notificationId);
                if (notification != null && !notification.IsAcknowledged)
                {
                    notification.IsAcknowledged = true;
                    notification.AcknowledgedAt = DateTime.Now;
                    await _unitOfWork.SaveAsync();

                    _logger.LogInformation("{ClassName}, {MethodName}, Notification acknowledged successfully. NotificationID: {NotificationID}", CLASSNAME, methodName, notificationId);
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Notification not found or already acknowledged. NotificationID: {NotificationID}", CLASSNAME, methodName, notificationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error acknowledging notification. NotificationID: {NotificationID}", CLASSNAME, methodName, notificationId);
                throw new ApplicationException($"Error acknowledging notification {notificationId}", ex);
            }
        }


    }
}
