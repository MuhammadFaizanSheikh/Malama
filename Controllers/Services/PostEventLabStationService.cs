using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class PostEventLabStationService : IPostEventLabStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<LabStationService> _logger;
        private const string CLASSNAME = "LabStationService";


        public PostEventLabStationService(ILogger<LabStationService> logger, IUnitOfWork unitOfWork, IMapper mapper, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<ResponseDto> AddAsync(PostEventLabStationDto model, string userName)
        {
            string methodName = nameof(AddAsync);

            try
            {
                var tokenResult = await _submissionTokenService
                    .ValidateAndSaveAsync(model.SubmissionToken, userName);

                if (!tokenResult.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Token validation failed. Message={Message}, User={User}",
                        CLASSNAME, methodName, tokenResult.Message, userName);

                    return tokenResult;
                }


                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Adding PostEventLabStation. ChildId={ChildId}, User={User}",
                    CLASSNAME, methodName, model.ServiceMembersChildId, userName);

                // Map DTO → Entity (now safe since profile is fixed)
                var postEventLabStation = _mapper.Map<PostEventLabStation>(model);

                // Safety checks (recommended even with mapping)
                if (postEventLabStation.ServiceMembersChildId == 0 ||
                    postEventLabStation.PostEventManagementId == 0)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid FK values. ChildId={ChildId}, PostEventManagementId={PostEventManagementId}",
                        CLASSNAME, methodName,
                        postEventLabStation.ServiceMembersChildId,
                        postEventLabStation.PostEventManagementId);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid reference data. Please reload and try again."
                    };
                }

                // Audit fields
                postEventLabStation.AddedOn = DateTime.Now;
                postEventLabStation.AddedBy = userName;

                await _unitOfWork.PostEventLabStation.AddAsync(postEventLabStation);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Record added successfully. Id={Id}, User={User}",
                    CLASSNAME, methodName, postEventLabStation.Id, userName);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Post Event Lab Station record added successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while adding PostEventLabStation. User={User}",
                    CLASSNAME, methodName, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Something went wrong while saving the record."
                };
            }
        }

        public async Task<ResponseDto> UpdateAsync(PostEventLabStationDto model, string userName)
        {
            string methodName = nameof(UpdateAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Updating PostEventLabStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                var existing = await _unitOfWork.PostEventLabStation.GetByIdAsync(model.Id);
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

                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Record updated successfully. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Post Event Lab Station record updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while updating PostEventLabStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model.Id, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Something went wrong while updating the record."
                };
            }
        }

        public async Task<PostEventLabStation?> GetByIdAsync(long id)
        {
            string methodName = nameof(GetByIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching PostEventLabStation by Id={Id}",
                    CLASSNAME, methodName, id);

                return await _unitOfWork.PostEventLabStation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception while fetching PostEventLabStation. Id={Id}",
                    CLASSNAME, methodName, id);

                return null;
            }
        }

        //public async Task UpdateAsync(LabStation model, string userName)
        //{
        //    string methodName = nameof(UpdateAsync);

        //    try
        //    {
        //        var existing = await _unitOfWork.LabStation.GetByIdAsync(model.Id);

        //        if (existing == null)
        //        {
        //            _logger.LogWarning("{ClassName}, {MethodName}, Lab Station record with Id={Id} not found by user {User}",
        //                CLASSNAME, methodName, model.Id, userName);

        //            throw new KeyNotFoundException($"Lab Station record with Id={model.Id} not found.");
        //        }

        //        MapToEntity(model, existing, userName);

        //        await _unitOfWork.SaveAsync();

        //        _logger.LogInformation("{ClassName}, {MethodName}, Lab Station record with Id={Id} successfully updated by user {User}",
        //            CLASSNAME, methodName, model.Id, userName);
        //    }
        //    catch (KeyNotFoundException knfEx)
        //    {
        //        _logger.LogError(knfEx,
        //            "{ClassName}, {MethodName}, KeyNotFoundException occurred while updating Lab Station record Id={Id} by user {User}",
        //            CLASSNAME, methodName, model.Id, userName);

        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "{ClassName}, {MethodName}, Exception occurred while updating Lab Station record Id={Id} by user {User}",
        //            CLASSNAME, methodName, model.Id, userName);

        //        throw;
        //    }
        //}

        //private void MapToEntity(LabStation source, LabStation target, string userName)
        //{
        //    //G6pd

        //    if (source.G6pdNeeded.IsNullOrEmpty())
        //    {
        //        target.G6pdGivenDateTime = null;
        //        target.G6pdReason = null;
        //    }
        //    else if (source.G6pdNeeded == "Not Completed")
        //    {
        //        target.G6pdGivenDateTime = null;
        //        target.G6pdReason = source.G6pdReason;
        //    }
        //    else
        //    {
        //        if (source.G6pdNeeded != target.G6pdNeeded)
        //        {
        //            target.G6pdGivenDateTime = DateTime.Now;
        //        }
        //    }

        //    //ABO 

        //    if (source.AboNeeded.IsNullOrEmpty())
        //    {
        //        target.AboGivenDateTime = null;
        //        target.AboGrouping = null;
        //        target.AboRhFactor = null;
        //        target.AboReason = null;
        //    }
        //    else if (source.AboNeeded == "Not Completed")
        //    {
        //        target.AboGivenDateTime = null;
        //        target.AboGrouping = null;
        //        target.AboRhFactor = null;
        //        target.AboReason = source.AboReason;
        //    }
        //    else
        //    {
        //        if (source.AboNeeded != target.AboNeeded)
        //        {
        //            target.AboGivenDateTime = DateTime.Now;
        //        }

        //        target.AboReason = null;
        //        target.AboGrouping = source.AboGrouping;
        //        target.AboRhFactor = source.AboRhFactor;
        //    }

        //    //Lipid

        //    if (source.LipidPanelNeeded.IsNullOrEmpty())
        //    {
        //        target.LipidPanelGivenDateTime = null;
        //        target.LipidPanelReason = null;
        //        target.LipidPanelRapidTesting = false;
        //        target.TotalCholesterol = null;
        //        target.TotalCholesterol_LessThan100 = false;
        //        target.TotalCholesterol_GreaterThan400 = false;
        //        target.HdlCholesterol = null;
        //        target.HdlCholesterol_LessThan20 = false;
        //        target.HdlCholesterol_GreaterThan120 = false;
        //        target.Triglycerides = null;
        //        target.Triglycerides_LessThan50 = false;
        //        target.Triglycerides_GreaterThan500 = false;
        //        target.Glucose = null;
        //        target.Glucose_LessThan20 = false;
        //        target.Glucose_GreaterThan600 = false;
        //        target.LdlCholesterol = null;
        //        target.TotalCholesterolHdlRatio = null;
        //        target.NonHdlCholesterol = null;
        //        target.LdlHdlLipoprotiens = null;
        //        target.A1C = null;
        //    }
        //    else if (source.LipidPanelNeeded == "Not Completed")
        //    {
        //        target.LipidPanelReason = source.LipidPanelReason;
        //        target.LipidPanelGivenDateTime = null;
        //        target.LipidPanelRapidTesting = false;
        //        target.TotalCholesterol = null;
        //        target.TotalCholesterol_LessThan100 = false;
        //        target.TotalCholesterol_GreaterThan400 = false;
        //        target.HdlCholesterol = null;
        //        target.HdlCholesterol_LessThan20 = false;
        //        target.HdlCholesterol_GreaterThan120 = false;
        //        target.Triglycerides = null;
        //        target.Triglycerides_LessThan50 = false;
        //        target.Triglycerides_GreaterThan500 = false;
        //        target.Glucose = null;
        //        target.Glucose_LessThan20 = false;
        //        target.Glucose_GreaterThan600 = false;
        //        target.LdlCholesterol = null;
        //        target.TotalCholesterolHdlRatio = null;
        //        target.NonHdlCholesterol = null;
        //        target.LdlHdlLipoprotiens = null;
        //        target.A1C = null;
        //    }
        //    else
        //    {
        //        if (source.LipidPanelNeeded != target.LipidPanelNeeded)
        //        {
        //            target.LipidPanelGivenDateTime = DateTime.Now;
        //        }

        //        if (source.LipidPanelRapidTesting)
        //        {
        //            target.LipidPanelRapidTesting = source.LipidPanelRapidTesting;
        //            target.TotalCholesterol = source.TotalCholesterol;
        //            target.TotalCholesterol_LessThan100 = source.TotalCholesterol_LessThan100;
        //            target.TotalCholesterol_GreaterThan400 = source.TotalCholesterol_GreaterThan400;
        //            target.HdlCholesterol = source.HdlCholesterol;
        //            target.HdlCholesterol_LessThan20 = source.HdlCholesterol_LessThan20;
        //            target.HdlCholesterol_GreaterThan120 = source.HdlCholesterol_GreaterThan120;
        //            target.Triglycerides = source.Triglycerides;
        //            target.Triglycerides_LessThan50 = source.Triglycerides_LessThan50;
        //            target.Triglycerides_GreaterThan500 = source.Triglycerides_GreaterThan500;
        //            target.Glucose = source.Glucose;
        //            target.Glucose_LessThan20 = source.Glucose_LessThan20;
        //            target.Glucose_GreaterThan600 = source.Glucose_GreaterThan600;
        //            target.LdlCholesterol = source.LdlCholesterol;
        //            target.TotalCholesterolHdlRatio = source.TotalCholesterolHdlRatio;
        //            target.NonHdlCholesterol = source.NonHdlCholesterol;
        //            target.LdlHdlLipoprotiens = source.LdlHdlLipoprotiens;
        //            target.A1C = source.A1C;
        //            target.LipidPanelReason = null;
        //        }
        //        else
        //        {
        //            target.LipidPanelRapidTesting = false;
        //            target.TotalCholesterol = null;
        //            target.TotalCholesterol_LessThan100 = false;
        //            target.TotalCholesterol_GreaterThan400 = false;
        //            target.HdlCholesterol = null;
        //            target.HdlCholesterol_LessThan20 = false;
        //            target.HdlCholesterol_GreaterThan120 = false;
        //            target.Triglycerides = null;
        //            target.Triglycerides_LessThan50 = false;
        //            target.Triglycerides_GreaterThan500 = false;
        //            target.Glucose = null;
        //            target.Glucose_LessThan20 = false;
        //            target.Glucose_GreaterThan600 = false;
        //            target.LdlCholesterol = null;
        //            target.TotalCholesterolHdlRatio = null;
        //            target.NonHdlCholesterol = null;
        //            target.LdlHdlLipoprotiens = null;
        //            target.A1C = null;
        //        }
        //    }

        //    //HIV

        //    if (source.HivNeeded.IsNullOrEmpty())
        //    {
        //        target.HivGivenDateTime = null;
        //        target.HivReason = null;
        //        target.HivBarcodeCarebill = null;
        //    }
        //    else if (source.HivNeeded == "Not Completed")
        //    {
        //        target.HivGivenDateTime = null;
        //        target.HivBarcodeCarebill = null;
        //        target.HivReason = source.HivReason;
        //    }
        //    else
        //    {
        //        if (source.HivNeeded != target.HivNeeded)
        //        {
        //            target.HivGivenDateTime = DateTime.Now;
        //        }

        //        target.HivReason = null;
        //        target.HivBarcodeCarebill = source.HivBarcodeCarebill;
        //    }

        //    //Pregnancy

        //    if (source.PregnancyTestNeeded.IsNullOrEmpty())
        //    {
        //        target.PregnancyTestGivenDateTime = null;
        //        target.PregnancyTestResult = null;
        //        target.PregnancyTestReason = null;
        //    }
        //    else if (source.PregnancyTestNeeded == "Not Completed")
        //    {
        //        target.PregnancyTestGivenDateTime = null;
        //        target.PregnancyTestResult = null;
        //        target.PregnancyTestReason = source.PregnancyTestReason;
        //    }
        //    else
        //    {
        //        if (source.PregnancyTestNeeded != target.PregnancyTestNeeded)
        //        {
        //            target.PregnancyTestGivenDateTime = DateTime.Now;
        //        }

        //        target.PregnancyTestResult = source.PregnancyTestResult;
        //        target.PregnancyTestReason = null;
        //    }

        //    target.AreYouFasting = source.AreYouFasting;
        //    target.AnyComplicationInBloodDrawn = source.AnyComplicationInBloodDrawn;
        //    target.AllergicToLatex = source.AllergicToLatex;
        //    target.FeelAlright = source.FeelAlright;
        //    target.G6pdNeeded = source.G6pdNeeded;
        //    target.AboNeeded = source.AboNeeded;
        //    target.LipidPanelNeeded = source.LipidPanelNeeded;
        //    target.HivNeeded = source.HivNeeded;
        //    target.PregnancyTestNeeded = source.PregnancyTestNeeded;
        //    target.FedExTrackingNo = source.FedExTrackingNo;
        //    target.Status = source.Status;
        //    target.UpdatedOn = DateTime.Now;
        //    target.UpdatedBy = userName;

            
        //}

        //public void SetGivenDateTimes(LabStation model)
        //{
        //    if (model == null) return;

        //    model.G6pdGivenDateTime = model.G6pdNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        //    model.AboGivenDateTime = model.AboNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        //    model.LipidPanelGivenDateTime = model.LipidPanelNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        //    model.HivGivenDateTime = model.HivNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        //    model.PregnancyTestGivenDateTime = model.PregnancyTestNeeded == "Completed" ? DateTime.Now : (DateTime?)null;
        //}
    }
}
