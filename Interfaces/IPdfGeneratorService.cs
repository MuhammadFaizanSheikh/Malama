using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> GenerateEventSummaryPdfAsync(FileDataDto dto);
    }
}
