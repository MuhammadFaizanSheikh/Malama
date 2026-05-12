using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record;
using System.Reflection;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class VitalStationService : IVitalStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IFileUploader _fileUploader;
        private readonly ILogger<VitalStationService> _logger;
        private readonly IEventManagementService _eventManagementService;
        private readonly IContractService _contractService;
        private const string CLASSNAME = "VitalStationService";


        public VitalStationService(ILogger<VitalStationService> logger, IUnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IPdfGeneratorService pdfGenerator, IEventManagementService eventManagementService, IFileUploader fileUploader, IContractService contractService)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _fileUploader = fileUploader;
            _eventManagementService = eventManagementService;
            _contractService = contractService;
            _logger = logger;
        }

        public async Task<VitalStationVM> GetVitalStationByServiceMemberChildIdAsync(long serviceMemberChildId)
        {
            const string methodName = "GetVitalStationByServiceMemberChildIdAsync";

            try
            {
                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMemberChildId,
                        c => c.ServiceMembersParent.EventManagement,
                        c => c.VitalStationRecord,
                        c => c.VitalStationRecord.BloodPressureReadings
                    )
                    .Select(c => new VitalStationVM
                    {
                        EventId = c.ServiceMembersParent.EventManagement.Id,
                        EventID = c.ServiceMembersParent.EventManagement.EventID,

                        ServiceMembersChild = new ServiceMembersChildDto
                        {
                            FullName = c.FullName,
                            DodId = c.DodId,
                            Barcode = c.Barcode,
                            Dob = c.Dob,
                            Age = c.Age,
                            Sex = c.Sex
                        },

                        VitalStationDto = c.VitalStationRecord == null
                            ? null
                            : new VitalStationDto
                            {
                                Id = c.VitalStationRecord.Id,
                                ServiceMembersChildId = c.VitalStationRecord.ServiceMembersChildId,
                                Height = c.VitalStationRecord.Height,
                                Weight = c.VitalStationRecord.Weight,
                                FinalBpStatus = c.VitalStationRecord.FinalBpStatus,
                                TotalReadingsTaken = c.VitalStationRecord.TotalReadingsTaken,
                                Status = c.VitalStationRecord.Status,

                                IsNextReadingRequired =
                                    c.VitalStationRecord.Status != "Completed"
                                    && c.VitalStationRecord.TotalReadingsTaken < 3,

                                NextReadingNumber = c.VitalStationRecord.TotalReadingsTaken + 1,

                                NextReadingAfterMinutes = 15,

                                Message = c.VitalStationRecord.Status == "Completed"
                                    ? "Vitals completed"
                                    : $"Reading {c.VitalStationRecord.TotalReadingsTaken + 1} required after 15 minutes",

                                BloodPressureReadings = c.VitalStationRecord.BloodPressureReadings
                                    .OrderBy(x => x.ReadingNumber)
                                    .Select(r => new VitalStationBpReadingDto
                                    {
                                        Id = r.Id,
                                        ReadingNumber = r.ReadingNumber,
                                        Systolic = r.Systolic,
                                        Diastolic = r.Diastolic,
                                        ReadingStatus = r.ReadingStatus,
                                        IsRetakeRequired = r.IsRetakeRequired,
                                        ReadingTakenAt = r.ReadingTakenAt,
                                        Remarks = r.Remarks
                                    })
                                    .ToList()
                            },

                        // You can extend VM if needed later
                    })
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching VitalStation for ChildId: {Id}", serviceMemberChildId);
                throw;
            }
        }

        public async Task AddAsync(VitalStationDto model, string userName)
        {
        }

        public async Task UpdateAsync(VitalStationDto model, string userName)
        {
        }
    }
}
