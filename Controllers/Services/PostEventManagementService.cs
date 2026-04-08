using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper;
using ExcelFilesCompiler.Utilities;

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




        //public async Task<List<EventManagementPreview>> GetAllEventID(bool includeVersion = true)
        //{
        //    const string methodName = "GetAllEventID";

        //    _logger.LogInformation("{ClassName}, {MethodName}, Called",
        //        CLASSNAME, methodName);

        //    try
        //    {
        //        // Fetch only the necessary data
        //        var result = await _unitOfWork.EventManagement
        //            .GetAllWithConditionNoTracking(e => e.EventStatus != "Canceled") // filtered in DB
        //            .OrderByDescending(e => e.Id)
        //            .Select(e => new EventManagementPreview
        //            {
        //                Id = e.Id,
        //                EventID = includeVersion
        //                    ? $"{e.EventID} (V{e.EventVersion})"
        //                    : e.EventID
        //            })
        //            .ToListAsync();

        //        _logger.LogInformation("{ClassName}, {MethodName}, Retrieved EventID count: {Count}",
        //            CLASSNAME, methodName, result.Count);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "{ClassName}, {MethodName}, Exception occurred while retrieving EventIDs",
        //            CLASSNAME, methodName);

        //        throw;
        //    }
        //}

        public async Task<PostEventManagementDto> GetById(long postEventManagementId)
        {
            var postEvent = await _unitOfWork.PostEventManagement
                .GetWithIncludeNoTracking(
                    x => x.Id == postEventManagementId,
                    x => x.PostEventStartEndTimeDayWise,
                    x => x.EventManagement.ContractDetails,
                    x => x.EventManagement.EventManagementTaskforcesList
                )
                .FirstOrDefaultAsync();

            if (postEvent == null)
                throw new Exception("PostEventManagement record not found.");

            var em = postEvent.EventManagement;

            return new PostEventManagementDto
            {
                Id = postEvent.Id,
                EventManagementId = postEvent.EventManagementId,
                EventStartDateUtc = postEvent.EventStartDateUtc,
                EventEndDateUtc = postEvent.EventEndDateUtc,
                PostEventNotes = postEvent.PostEventNotes,

                PostEventStartEndTimeDayWiseDto = postEvent.PostEventStartEndTimeDayWise?
                    .Select(d => new PostEventStartEndTimeDayWiseDto
                    {
                        Id = d.Id,
                        EventDay = d.EventDay,
                        EventStartTime = d.EventStartTime,
                        EventEndTime = d.EventEndTime,
                        ServiceMemberPercentPerDay = d.ServiceMemberPercentPerDay
                    })
                    .ToList(),

                // Map only the specific fields of ContractDetails
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

                // EventManagement fields
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
                TotalServiceMember = em.TotalRequestedServiceMembers,
                TaskForce = em?.EventManagementTaskforcesList != null
                    ? string.Join(", ", em.EventManagementTaskforcesList.Select(t => t.Taskforce))
                    : null
            };
        }
    }
}
