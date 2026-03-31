using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ExcelFilesCompiler.UnitOfWork;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class UserEventMappingService : IUserEventMappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserEventMappingService> _logger;
        private const string CLASSNAME = nameof(UserEventMappingService);

        public UserEventMappingService(IUnitOfWork unitOfWork, ILogger<UserEventMappingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> IsUserAssignedToEventAsync(string userId, long eventId)
        {
            const string methodName = nameof(IsUserAssignedToEventAsync);
            _logger.LogInformation("{ClassName}, {MethodName}, Checking if user {UserId} is assigned to event {EventId}",
                CLASSNAME, methodName, userId, eventId);

            try
            {
                bool isAssigned = await _unitOfWork.UserEventMapping
                    .GetAllWithConditionNoTracking(u => u.UserId == userId && u.EventId == eventId)
                    .AnyAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, User assignment result: {IsAssigned}",
                    CLASSNAME, methodName, isAssigned);

                return isAssigned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to check user assignment for event {EventId}",
                    CLASSNAME, methodName, eventId);
                throw;
            }
        }

        public async Task<List<long>> GetEventsAgainstUserId(string userId)
        {
            const string methodName = nameof(GetEventsAgainstUserId);
            _logger.LogInformation("{ClassName}, {MethodName}, Getting events against UserId : {userId}",
                CLASSNAME, methodName, userId);

            try
            {
                var eventIds = await _unitOfWork.UserEventMapping.GetAllWithConditionNoTracking(x => x.UserId == userId)
                .Select(x => x.EventId).ToListAsync();

                if (!eventIds.Any())
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No events found for UserId : {UserId}",
                        CLASSNAME, methodName, userId
                    );
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {EventCount} events against UserId : {UserId}",
                    CLASSNAME, methodName, eventIds.Count, userId);
                }

                return eventIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to get events against UserId : {userId}",
                    CLASSNAME, methodName, userId);
                throw;
            }
        }
    }


}
