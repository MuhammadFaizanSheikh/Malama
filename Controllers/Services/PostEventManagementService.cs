using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper;
using ExcelFilesCompiler.Utilities;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class PostEventManagementService : IPostEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<PostEventManagementService> _logger;
        private const string CLASSNAME = "ContractService";

        public PostEventManagementService(ILogger<PostEventManagementService> logger, IMapper mapper, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, ISubmissionTokenService submissionTokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        //public async Task<PostEventManagementDto> GetById(long postEventManagementId)
        //{
        //    var postEvent = await _unitOfWork.PostEventManagement
        //    .GetWithIncludeNoTracking(
        //        x => x.Id == postEventManagementId,
        //        x => x.PostEventStartEndTimeDayWise,
        //        x => x.EventManagement.ContractDetails,
        //        x => x.EventManagement.EventManagementTaskforcesList,
        //        x => x.EventManagement.EventServiceDetailList,
        //        x => x.PostEventServiceDetails,
        //        x => x.PostEventServiceDetails.Select(psd => psd.EventServiceDetail) // ⭐ CRITICAL FIX
        //    )
        //    .FirstOrDefaultAsync();

        //    if (postEvent == null)
        //        throw new Exception("PostEventManagement record not found.");

        //    var em = postEvent.EventManagement;

        //    return new PostEventManagementDto
        //    {
        //        Id = postEvent.Id,
        //        EventManagementId = postEvent.EventManagementId,
        //        EventStartDateUtc = postEvent.EventStartDateUtc,
        //        EventEndDateUtc = postEvent.EventEndDateUtc,
        //        PostEventNotes = postEvent.PostEventNotes,

        //        PostEventStartEndTimeDayWiseDto = postEvent.PostEventStartEndTimeDayWise?
        //            .Select(d => new PostEventStartEndTimeDayWiseDto
        //            {
        //                Id = d.Id,
        //                EventDay = d.EventDay,
        //                EventStartTime = d.EventStartTime,
        //                EventEndTime = d.EventEndTime,
        //                ServiceMemberPercentPerDay = d.ServiceMemberPercentPerDay
        //            })
        //            .ToList(),
        //        EventServices = postEvent.PostEventServiceDetails != null
        //        ? postEvent.PostEventServiceDetails
        //            .Select(x => new PostEventServiceDetailDto
        //            {
        //                EventServiceDetailId = x.EventServiceDetailId,
        //                EventService = x.EventServiceDetail.EventService, // requires include
        //                FinalPreEventConfirmedNumbers = x.EventServiceDetail.FinalPreEventConfirmedNumbers,
        //                Completed = x.Completed,
        //                PostEventNumbers= x.PostEventNumbers
        //            }).ToList()
        //        : new List<PostEventServiceDetailDto>(),
        //        // Map only the specific fields of ContractDetails
        //        ContractDetails = em?.ContractDetails == null
        //            ? null
        //            : new ContractDetails
        //            {
        //                Id = em.ContractDetails.Id,
        //                ContractID = em.ContractDetails.ContractID,
        //                ContractName = em.ContractDetails.ContractName,
        //                ContractClient = em.ContractDetails.ContractClient,
        //                SiteId = em.ContractDetails.SiteId,
        //                ContractAgency = em.ContractDetails.ContractAgency,
        //                ContractComponent = em.ContractDetails.ContractComponent,
        //                ClientName = em.ContractDetails.ClientName,
        //                ContractType = em.ContractDetails.ContractType,
        //                ContractStartDate = em.ContractDetails.ContractStartDate,
        //                ContractEndDate = em.ContractDetails.ContractEndDate,
        //                DawsonProjectManagerFirstName = em.ContractDetails.DawsonProjectManagerFirstName,
        //                ContractServiceBranch = em.ContractDetails.ContractServiceBranch
        //            },

        //        // EventManagement fields
        //        SubEventID = em?.SubEventID,
        //        EventAddress1 = em?.EventAddress1,
        //        EventAddress2 = em?.EventAddress2,
        //        EventState = em?.EventState,
        //        EventCity = em?.EventCity,
        //        EventZipCode = em?.EventZipCode,
        //        MOBDate = em?.MOBDate,
        //        RegardingSites = em?.RegardingSites,
        //        Timezone = em?.Timezone,
        //        EventHelpLine = em?.EventHelpLine,
        //        TotalServiceMember = em.TotalRequestedServiceMembers,
        //        TaskForce = em?.EventManagementTaskforcesList != null
        //            ? string.Join(", ", em.EventManagementTaskforcesList.Select(t => t.Taskforce))
        //            : null
        //    };
        //}

        public async Task<(PostEventManagementDto Data, string EventID)> GetById(long postEventManagementId)
        {
            const string methodName = nameof(GetById);

            _logger.LogInformation("{ClassName}, {MethodName}, Called. Id={Id}",
                CLASSNAME, methodName, postEventManagementId);

            try
            {
                var postEvent = await _unitOfWork.PostEventManagement
                .GetWithIncludeNoTracking(
                    x => x.Id == postEventManagementId,
                    x => x.PostEventStartEndTimeDayWise,
                    x => x.EventManagement.ContractDetails,
                    x => x.EventManagement.EventManagementTaskforcesList,
                    x => x.EventManagement.EventServiceDetailList,
                    x => x.PostEventServiceDetails.OrderBy(p => p.EventServiceDetailId)
                )
                .Include(x => x.EventManagement.ServiceMembersParents)
                    .ThenInclude(p => p.ServiceMembersChildren)
                    .AsSplitQuery()
                .FirstOrDefaultAsync();

                if (postEvent == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Record not found. Id={Id}",
                        CLASSNAME, methodName, postEventManagementId);

                    throw new Exception("PostEventManagement record not found.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Event found. Checking ServiceMembersChildren count",
                    CLASSNAME, methodName);

                int totalServiceMembers = postEvent?.EventManagement?.ServiceMembersParents?
                .SelectMany(p => p.ServiceMembersChildren ?? new List<ServiceMembersChild>())
                .Count() ?? 0;

                _logger.LogInformation("{ClassName}, {MethodName}, ServiceMembersChildren count : {totalServiceMembers}",
                    CLASSNAME, methodName, totalServiceMembers);

                // ✅ Load missing EventServiceDetail (manual include fix)
                var serviceDetailIds = postEvent.PostEventServiceDetails?
                    .Select(x => x.EventServiceDetailId)
                    .ToList() ?? new List<long>();

                var serviceDetails = serviceDetailIds.Any()
                    ? await _unitOfWork.EventServiceDetail
                        .GetWithIncludeNoTracking(x => serviceDetailIds.Contains(x.Id))
                        .ToListAsync()
                    : new List<EventServiceDetail>();

                foreach (var item in postEvent.PostEventServiceDetails)
                {
                    item.EventServiceDetail = serviceDetails
                        .FirstOrDefault(x => x.Id == item.EventServiceDetailId);
                }

                var em = postEvent.EventManagement;

                _logger.LogInformation("{ClassName}, {MethodName}, Converting timezone. Timezone={Timezone}",
                    CLASSNAME, methodName, em?.Timezone);

                Helper.ConvertEventToLocalTime(postEvent, em.Timezone);
                Helper.ConvertEventToLocalTime(em, em.Timezone);

                var dto = new PostEventManagementDto
                {
                    Id = postEvent.Id,
                    EventManagementId = postEvent.EventManagementId,

                    EventStartDateUtc = postEvent.EventStartDateUtc,
                    EventEndDateUtc = postEvent.EventEndDateUtc,

                    PostEventNotes = postEvent.PostEventNotes,
                    PostEventStatus = postEvent.PostEventStatus,
                    TotalServiceMember = totalServiceMembers,

                    // ✅ Day-wise mapping
                    PostEventStartEndTimeDayWiseDto = postEvent.PostEventStartEndTimeDayWise != null
                        ? postEvent.PostEventStartEndTimeDayWise.Select(d => new PostEventStartEndTimeDayWiseDto
                        {
                            Id = d.Id,
                            EventDay = d.EventDay,
                            EventStartTime = d.EventStartTime,
                            EventEndTime = d.EventEndTime,
                            ServiceMemberPercentPerDay = d.ServiceMemberPercentPerDay
                        }).ToList()
                        : new List<PostEventStartEndTimeDayWiseDto>(),

                    // ✅ Services mapping
                    EventServices = postEvent.PostEventServiceDetails != null
                        ? postEvent.PostEventServiceDetails.Select(x => new PostEventServiceDetailDto
                        {
                            Id = x.Id,
                            EventServiceDetailId = x.EventServiceDetailId,
                            EventService = x.EventServiceDetail?.EventService,
                            FinalPreEventConfirmedNumbers = x.EventServiceDetail?.FinalPreEventConfirmedNumbers,
                            Completed = x.Completed,
                            PostEventNumbers = x.PostEventNumbers
                        }).ToList()
                        : new List<PostEventServiceDetailDto>(),

                    // ✅ Contract details (FULL)
                    ContractDetails = em?.ContractDetails == null
                        ? null
                        : new ContractDetails
                        {
                            Id = em.ContractDetails.Id,
                            ContractID = em.ContractDetails.ContractID,
                            ContractName = em.ContractDetails.ContractName,
                            ContractClient = em.ContractDetails.ContractClient,
                            SiteId = em.ContractDetails.SiteId,
                            ContractAgency = em.ContractDetails.ContractAgency,
                            ContractComponent = em.ContractDetails.ContractComponent,
                            ClientName = em.ContractDetails.ClientName,
                            ContractType = em.ContractDetails.ContractType,
                            ContractStartDate = em.ContractDetails.ContractStartDate,
                            ContractEndDate = em.ContractDetails.ContractEndDate,
                            DawsonProjectManagerFirstName = em.ContractDetails.DawsonProjectManagerFirstName,
                            ContractServiceBranch = em.ContractDetails.ContractServiceBranch
                        },

                    // ✅ Event fields (FULL)
                    SubEventID = em?.SubEventID,
                    EventAddress1 = em?.EventAddress1,
                    EventAddress2 = em?.EventAddress2,
                    EventState = em?.EventState,
                    EventCity = em?.EventCity,
                    EventZipCode = em?.EventZipCode,
                    MOBDate = em?.MOBDate,
                    RegardingSites = em?.RegardingSites,
                    Timezone = em?.Timezone,
                    EventHelpLine = em?.EventHelpLine,
                    EventStartDateUtcForDisplay = em.EventStartDateUtc,
                    EventEndDateUtcForDisplay = em.EventEndDateUtc,
                    TaskForce = em?.EventManagementTaskforcesList != null
                        ? string.Join(", ", em.EventManagementTaskforcesList.Select(t => t.Taskforce))
                        : null
                };

                _logger.LogInformation("{ClassName}, {MethodName}, DTO prepared successfully. Id={Id}",
                    CLASSNAME, methodName, postEventManagementId);

                return (dto, em.EventID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred. Id={Id}",
                    CLASSNAME, methodName, postEventManagementId);

                throw;
            }
        }

        public async Task<ResponseDto> AddAsync(PostEventManagementDto model, string userName)
        {
            string methodName = nameof(AddAsync);

            try
            {
                var tokenResult = await _submissionTokenService
                    .ValidateAndSaveAsync(model.SubmissionToken, userName);

                // ✅ HANDLE TOKEN FAILURE
                if (!tokenResult.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Token validation failed. Message={Message}, User={User}",
                        CLASSNAME, methodName, tokenResult.Message, userName);

                    return tokenResult;
                }

                PostEventManagement postEventManagement = new();
                _mapper.Map(model, postEventManagement);

                postEventManagement.AddedOn = DateTime.Now;
                postEventManagement.AddedBy = userName;

                var responseDto = Helper.ConvertEventTimesToUtc(postEventManagement, model.Timezone);

                // (optional) if this also returns response
                // if (!responseDto.Success) return responseDto;

                await _unitOfWork.PostEventManagement.AddAsync(postEventManagement);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Record added successfully. Id={Id}, User={User}",
                    CLASSNAME, methodName, postEventManagement.Id, userName);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Post Event Management record added successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while adding record. User={User}",
                    CLASSNAME, methodName, userName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Something went wrong while saving the record."
                };
            }
        }

        public async Task UpdateAsync(PostEventManagementDto model, string userName)
        {
            string methodName = nameof(UpdateAsync);

            try
            {
                var existing = await _unitOfWork.PostEventManagement
                    .GetWithIncludeTracking(e => e.Id == model.Id,
                        e => e.PostEventServiceDetails,
                        e => e.PostEventStartEndTimeDayWise)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Post Event Management record with Id={Id} not found by user {User}",
                        CLASSNAME, methodName, model.Id, userName);

                    throw new KeyNotFoundException($"Post Event Management record with Id={model.Id} not found.");
                }


                string addedBy = existing.AddedBy;
                DateTime addedOn = existing.AddedOn;

                _mapper.Map(model, existing);
                existing.AddedBy = addedBy;
                existing.AddedOn = addedOn;
                existing.UpdatedBy = userName;
                existing.UpdatedOn = DateTime.Now;

                var responseDto = Helper.ConvertEventTimesToUtc(existing, model.Timezone);

                //if (!responseDto.Success) return responseDto;

                Helper.UpdateCollection(
                    existing.PostEventServiceDetails,
                    model.EventServices.Select(dto => new PostEventServiceDetail
                    {
                        Id = dto.Id,
                        EventServiceDetailId = dto.EventServiceDetailId,
                        PostEventNumbers = dto.PostEventNumbers,
                        Completed = dto.Completed,
                        PostEventManagementId = existing.Id
                    }).ToList(),
                    x => x.Id,
                    _mapper
                );

                Helper.UpdateCollection(
                    existing.PostEventStartEndTimeDayWise,
                    model.PostEventStartEndTimeDayWiseDto.Select(dto => new PostEventStartEndTimeDayWise
                    {
                        Id = dto.Id,
                        EventDay = dto.EventDay,
                        EventStartTime = dto.EventStartTime,
                        EventEndTime = dto.EventEndTime,
                        PostEventManagementId = existing.Id
                    }).ToList(),
                    x => x.Id,
                    _mapper
                );

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Post Event Management record with Id={Id} successfully updated by user {User}",
                    CLASSNAME, methodName, model.Id, userName);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogError(knfEx,
                    "{ClassName}, {MethodName}, KeyNotFoundException occurred while updating Post Event Management record Id={Id} by user {User}",
                    CLASSNAME, methodName, model.Id, userName);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while updating Immunization record Id={Id} by user {User}",
                    CLASSNAME, methodName, model.Id, userName);

                throw;
            }
        }
    }
}
