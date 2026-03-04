using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISubmissionTokenService
    {
        Task<ResponseDto> ValidateAndSaveAsync(string submissionToken, string userName);
    }
}
