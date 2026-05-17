using Malama.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IFileUploader
    {
        List<List<Dictionary<string, object>>> UploadAndPreview(List<IFormFile> files, IFormFile G6PDFile, DateTime parsedEventDate, DateTime? parsedLastEventDate, long eventId, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing);
        Task<ResponseDto> CheckForExistingDataAgainstEventIdAsync(string eventId, string addedBy);
        Task<ResponseDto> AddRecordsBulkAsync(List<FileDataDto> fileDataDtos, string eventId, string addedBy);
        //Task<List<string>> GetDistinctEventIdsAsync();
        Task<List<ServiceMembersChild>> GetImmunizationsByEventIdAsync(long eventId);
        Task<List<ServiceMembersChild>> GetLabStationByEventIdAsync(long eventId, string? status = null);
        Task<List<ServiceMembersChild>> GetVitalStationByEventIdAsync(long eventId);
        Task<PostEventLabStationAnalysisDto?> GetPostEventLabStationAnalysisDtoAsync(long serviceMembersChildId);
        Task<List<ServiceMembersChild>> GetPreAndPostLabStationByEventIdAsync(long eventId);
        Task<List<ServiceMembersChild>> GetPreAndPostImmunizationStationByEventIdAsync(long eventId);
        Task<PostEventImmunizationStationAnalysisDto?> GetPostEventImmunizationStationAnalysisDtoAsync(long serviceMembersChildId);
        Task<List<ServiceMembersChild>> GetEventDataByEventIdForLabHivReport(long eventId);
        Task<ResponseDto> AddSingleRecordAsync(FileDataDto dto, long eventId, string addedBy);
        Task<ResponseDto> UpdateSingleRecordAsync(FileDataDto dto, string addedBy);
        //Task<ServiceMembersChild> GetByIdAsync(long id);
        Task<(ServiceMembersChild ServiceMembersChild, long EventId)> GetServiceMemberChildWithEventIdAsync(long serviceMemberChildId);
        Task<ServiceMembersChild> GetByIdWithInclude(long id);
    }
}
