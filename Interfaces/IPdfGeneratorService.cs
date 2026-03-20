using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateEventSummaryPdfAsync(ServiceMembersChild dto);
        Task<byte[]> GenerateHivSignInSheetPdfAsync(List<ServiceMembersChild> dtos, EventManagement eventInfo, ContractDetails contractDetail);
    }
}
