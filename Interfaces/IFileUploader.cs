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
        Task<List<ImmunizationStation>> GetImmunizationsByEventIdAsync(string eventId);
        Task<List<LabStation>> GetLabStationByEventIdAsync(string eventId);
        Task<List<ServiceMembersChild>> GetEventDataByEventIdForLabHivReport(string eventId);
        Task<ResponseDto> AddSingleRecordAsync(FileDataDto dto, string eventId, int eventVersion, string addedBy);
        Task<ResponseDto> UpdateSingleRecordAsync(FileDataDto dto, string addedBy);
        //Task<ServiceMembersChild> GetByIdAsync(long id);
        Task<(ServiceMembersChild ServiceMembersChild, string EventId)> GetServiceMemberChildWithEventIdAsync(long serviceMemberChildId);
        Task<ServiceMembersChild> GetByIdWithInclude(long id);
    }
}
