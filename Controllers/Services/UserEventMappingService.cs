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
                    .FindForSearching(u => u.UserId == userId && u.EventId == eventId)
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
    }


}
