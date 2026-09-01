using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalCoordinatorStationService
    {
        /// <summary>
        /// Saves all Treatment Coordinator station sections in one DB transaction.
        /// X-Ray files are staged first, committed only after DB commit; staging is rolled back on failure.
        /// Add future section applies here with saveChanges:false before the shared SaveAsync/Commit.
        /// </summary>
        Task<DentalCoordinatorStationSaveResult> SaveStationAsync(
            DentalCoordinatorStationSaveDto dto,
            ServiceMembersChild serviceMember,
            string userName,
            string userId);
    }

    public class DentalCoordinatorStationSaveResult
    {
        public bool Success { get; set; }
        public string? ErrorTitle { get; set; }
        public string? ErrorMessage { get; set; }

        public static DentalCoordinatorStationSaveResult Ok() => new() { Success = true };

        public static DentalCoordinatorStationSaveResult Fail(string title, string message) => new()
        {
            Success = false,
            ErrorTitle = title,
            ErrorMessage = message
        };
    }
}
