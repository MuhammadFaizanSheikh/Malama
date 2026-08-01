using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Malama.Utilities
{
    public static class DentalExamSignatureHelper
    {
        public static async Task<(string DisplayName, string Roles)> ResolveDisplayAsync(
            bool signatureEntered,
            string? signedUserId,
            ApplicationUser? currentUserForPreview,
            long? eventManagementId,
            UserManager<ApplicationUser> userManager,
            IEventStaffService eventStaffService,
            ILogger logger)
        {
            ApplicationUser? user = null;

            if (signatureEntered && !string.IsNullOrWhiteSpace(signedUserId))
            {
                user = await userManager.FindByIdAsync(signedUserId);
            }
            else if (!signatureEntered)
            {
                user = currentUserForPreview;
            }

            if (user == null)
            {
                return (string.Empty, string.Empty);
            }

            var displayName = await ResolveDisplayNameAsync(user, eventStaffService, logger);
            var roles = await ResolveEventWiseRolesAsync(user.Id, eventManagementId, eventStaffService, logger);
            return (displayName, roles);
        }

        public static async Task<string> ResolveDisplayNameAsync(
            ApplicationUser? user,
            IEventStaffService eventStaffService,
            ILogger logger)
        {
            if (user == null)
            {
                return string.Empty;
            }

            if (!user.IsEventUser)
            {
                return user.UserName?.Trim() ?? string.Empty;
            }

            try
            {
                var staff = await eventStaffService.GetEventStaffWithAttributesByUserId(user.Id);
                return FormatEventStaffDisplayName(staff);
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning(
                    "DentalExamSignatureHelper.ResolveDisplayNameAsync, EventStaff not found for UserId={UserId}. Falling back to UserName.",
                    user.Id);
                return user.UserName?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "DentalExamSignatureHelper.ResolveDisplayNameAsync, Failed to resolve EventStaff name for UserId={UserId}",
                    user.Id);
                return user.UserName?.Trim() ?? string.Empty;
            }
        }

        public static async Task<string> ResolveEventWiseRolesAsync(
            string userId,
            long? eventManagementId,
            IEventStaffService eventStaffService,
            ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return string.Empty;
            }

            if (!eventManagementId.HasValue || eventManagementId.Value <= 0)
            {
                logger.LogWarning(
                    "DentalExamSignatureHelper.ResolveEventWiseRolesAsync, EventManagementId missing for UserId={UserId}",
                    userId);
                return string.Empty;
            }

            try
            {
                var roleNames = await eventStaffService.GetEventAssignedRoleNamesByUserIdAsync(
                    userId,
                    eventManagementId.Value);

                return string.Join(Environment.NewLine, roleNames);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "DentalExamSignatureHelper.ResolveEventWiseRolesAsync failed for UserId={UserId}, EventManagementId={EventManagementId}",
                    userId, eventManagementId);
                return string.Empty;
            }
        }

        public static async Task<Dictionary<string, string>> ResolveExaminerNamesByUserIdAsync(
            IEnumerable<DentalExamFinding>? findings,
            UserManager<ApplicationUser> userManager,
            IEventStaffService eventStaffService,
            ILogger logger)
        {
            var names = new Dictionary<string, string>(StringComparer.Ordinal);
            var userIds = (findings ?? Enumerable.Empty<DentalExamFinding>())
                .SelectMany(f => new[] { f.ExaminationAddedBy, f.ExaminationUpdatedBy })
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            foreach (var userId in userIds)
            {
                var user = await userManager.FindByIdAsync(userId!);
                names[userId!] = await ResolveDisplayNameAsync(user, eventStaffService, logger);
            }

            return names;
        }

        public static string FormatEventStaffDisplayName(EventStaff staff)
        {
            return string.Join(" ",
                new[] { staff.StaffFirstName, staff.StaffMiddleInitial, staff.StaffLastName }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part!.Trim()));
        }

        public static long? TryResolveEventManagementId(ClaimsPrincipal user, ISession? session, long? fallbackEventId = null)
        {
            if (fallbackEventId.HasValue && fallbackEventId.Value > 0)
            {
                return fallbackEventId.Value;
            }

            var claimValue = user.FindFirst("EventIdLong")?.Value;
            if (long.TryParse(claimValue, out var claimEventId) && claimEventId > 0)
            {
                return claimEventId;
            }

            var sessionValue = session?.GetString("GlobalEventIdLong");
            if (long.TryParse(sessionValue, out var sessionEventId) && sessionEventId > 0)
            {
                return sessionEventId;
            }

            return null;
        }
    }
}
