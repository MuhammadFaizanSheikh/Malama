using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateEventSummaryPdfAsync(FileDataDto dto);
        Task<byte[]> GenerateHivSignInSheetPdfAsync(List<FileDataDto> dtos, EventManagement eventInfo, ContractDetails contractDetail);
    }
}
