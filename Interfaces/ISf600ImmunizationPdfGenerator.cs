using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISf600ImmunizationPdfGenerator
    {
        Task<byte[]> GenerateAsync(PostEventImmunizationStationAnalysisDto analysisDto);
    }
}
