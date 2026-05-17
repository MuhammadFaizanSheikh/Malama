using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class PostEventImmunizationStationService : IPostEventImmunizationStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostEventImmunizationStationService> _logger;
        private const string CLASSNAME = nameof(PostEventImmunizationStationService);

        public PostEventImmunizationStationService(
            ILogger<PostEventImmunizationStationService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<ResponseDto> AddAsync(PostEventImmunizationStationDto model, string userName)
        {
            string methodName = nameof(AddAsync);

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(model.SubmissionToken, userName);
                if (!tokenResult.Success)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Token validation failed. Message={Message}, User={User}",
                        CLASSNAME, methodName, tokenResult.Message, userName);
                    return tokenResult;
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Adding PostEventImmunizationStation. ChildId={ChildId}, User={User}",
                    CLASSNAME, methodName, model.ServiceMembersChildId, userName);

                var entity = _mapper.Map<PostEventImmunizationStation>(model);

                if (entity.ServiceMembersChildId == 0 || entity.PostEventManagementId == 0)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid FK values. ChildId={ChildId}, PostEventManagementId={PostEventManagementId}",
                        CLASSNAME, methodName, entity.ServiceMembersChildId, entity.PostEventManagementId);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid reference data. Please reload and try again."
                    };
                }

                entity.AddedOn = DateTime.Now;
                entity.AddedBy = userName;

                await ApplyDataEnteredAndStatusAsync(entity);

                await _unitOfWork.PostEventImmunizationStation.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Record added successfully. Id={Id}, User={User}",
                    CLASSNAME, methodName, entity.Id, userName);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Post Event Immunization record added successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while adding PostEventImmunizationStation. User={User}",
                    CLASSNAME, methodName, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Something went wrong while saving the record."
                };
            }
        }

        public async Task<ResponseDto> UpdateAsync(PostEventImmunizationStationDto model, string userName)
        {
            string methodName = nameof(UpdateAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Updating PostEventImmunizationStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                var existing = await _unitOfWork.PostEventImmunizationStation.GetByIdAsync(model.Id);
                if (existing == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Record not found for update. Id={Id}, User={User}",
                        CLASSNAME, methodName, model.Id, userName);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Record not found."
                    };
                }

                _mapper.Map(model, existing);
                existing.UpdatedOn = DateTime.Now;
                existing.UpdatedBy = userName;

                await ApplyDataEnteredAndStatusAsync(existing);

                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Record updated successfully. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Post Event Immunization record updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while updating PostEventImmunizationStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Something went wrong while updating the record."
                };
            }
        }

        public async Task<PostEventImmunizationStation?> GetByIdAsync(long id)
        {
            string methodName = nameof(GetByIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching PostEventImmunizationStation by Id={Id}",
                    CLASSNAME, methodName, id);

                return await _unitOfWork.PostEventImmunizationStation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while fetching PostEventImmunizationStation. Id={Id}",
                    CLASSNAME, methodName, id);

                return null;
            }
        }

        private async Task ApplyDataEnteredAndStatusAsync(PostEventImmunizationStation entity)
        {
            NormalizeDataEnteredFields(entity);

            var preEvent = await _unitOfWork.ServiceMembersChild
                .GetWithIncludeNoTracking(c => c.Id == entity.ServiceMembersChildId, c => c.ImmunizationRecord)
                .Select(c => c.ImmunizationRecord)
                .FirstOrDefaultAsync();

            entity.Status = ComputeStatus(entity, preEvent);
        }

        private static void NormalizeDataEnteredFields(PostEventImmunizationStation entity)
        {
            entity.HepBDataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.HepBDataEntered, entity.HepBDataEnteredDateTime);
            entity.HepADataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.HepADataEntered, entity.HepADataEnteredDateTime);
            entity.FluDataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.FluDataEntered, entity.FluDataEnteredDateTime);
            entity.MmrDataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.MmrDataEntered, entity.MmrDataEnteredDateTime);
            entity.TetTdpDataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.TetTdpDataEntered, entity.TetTdpDataEnteredDateTime);
            entity.VaricellaDataEnteredDateTime = NormalizeVaccineDataEnteredDateTime(entity.VaricellaDataEntered, entity.VaricellaDataEnteredDateTime);
        }

        private static DateTime? NormalizeVaccineDataEnteredDateTime(bool dataEntered, DateTime? dateTime)
        {
            if (!dataEntered)
            {
                return null;
            }

            return dateTime ?? DateTime.Now;
        }

        private static string ComputeStatus(PostEventImmunizationStation entity, ImmunizationStation? preEvent)
        {
            if (preEvent == null)
            {
                return AppConstants.Status.Pending;
            }

            if (IsVaccinePending(preEvent.HepBNeeded, entity.HepBDataEntered) ||
                IsVaccinePending(preEvent.HepANeeded, entity.HepADataEntered) ||
                IsVaccinePending(preEvent.FluNeeded, entity.FluDataEntered) ||
                IsVaccinePending(preEvent.MMRNeeded, entity.MmrDataEntered) ||
                IsVaccinePending(preEvent.TetTdpNeeded, entity.TetTdpDataEntered) ||
                IsVaccinePending(preEvent.VaricellaNeeded, entity.VaricellaDataEntered))
            {
                return AppConstants.Status.Pending;
            }

            return AppConstants.Status.Completed;
        }

        private static bool IsVaccinePending(string? needed, bool dataEntered) =>
            needed == AppConstants.Status.Completed && !dataEntered;
    }
}
