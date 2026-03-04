using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class SubmissionTokenService : ISubmissionTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubmissionTokenService> _logger;
        private const string CLASSNAME = nameof(SubmissionTokenService);

        public SubmissionTokenService(IUnitOfWork unitOfWork, ILogger<SubmissionTokenService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto> ValidateAndSaveAsync(string submissionToken, string userName)
        {
            const string methodName = "ValidateAndSaveAsync";

            try
            {
                var existingToken = await _unitOfWork.SubmissionTokenRecord.FindAsync(t => t.Token == submissionToken);

                if (existingToken != null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Duplicate submission token detected: {Token}, User: {UserName}",
                        CLASSNAME, methodName, submissionToken, userName);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "This form has already been submitted."
                    };
                }

                // Save the token if not duplicate
                await _unitOfWork.SubmissionTokenRecord.AddAsync(new SubmissionTokenRecord
                {
                    Token = submissionToken,
                    CreatedAt = DateTime.Now
                });

                _logger.LogInformation("{ClassName}, {MethodName}, Submission token saved: {Token}, User: {UserName}",
                    CLASSNAME, methodName, submissionToken, userName);

                return new ResponseDto { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error validating submission token: {Token}, User: {UserName}",
                    CLASSNAME, methodName, submissionToken, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = $"Error validating submission token: {ex.Message}"
                };
            }
        }
    }
}
