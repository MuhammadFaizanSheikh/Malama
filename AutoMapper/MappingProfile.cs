using AutoMapper;
using Malama.Models;

namespace Malama.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 🔹 ServiceMemberChild (Entity → Entity)
            CreateMap<FileDataDto, ServiceMembersChild>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CheckInTime,
        opt => opt.MapFrom(src => Malama.Utilities.Helper.NormalizeDateTime(src.CheckInTime)))
    .ForMember(dest => dest.CheckOutTime,
        opt => opt.MapFrom(src => Malama.Utilities.Helper.NormalizeDateTime(src.CheckOutTime)));

            // 🔹 Contract (Entity → Entity)
            CreateMap<ContractDetails, ContractDetails>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.EventManagement, opt => opt.Ignore()); // navigation

            // 🔹 SubContractor (Entity → Entity)
            CreateMap<SubContractor, SubContractor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // PK should not change
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceTypeProvided, opt => opt.Ignore());

            // 🔹 EventManagement (Entity → Entity)
            CreateMap<EventManagement, EventManagement>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ContractDetails, opt => opt.Ignore()) // if exists
                .ForMember(dest => dest.EventServiceDetailList, opt => opt.Ignore())
                .ForMember(dest => dest.EventStartEndTimeDayWiseList, opt => opt.Ignore())
                .ForMember(dest => dest.EventStaffDetailList, opt => opt.Ignore())
                .ForMember(dest => dest.EventManagementTaskforcesList, opt => opt.Ignore());

            // 🔹 Child Entities (VERY IMPORTANT for UpdateCollection)
            CreateMap<EventServiceDetail, EventServiceDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventManagementId, opt => opt.Ignore());
            CreateMap<EventStartEndTimeDayWise, EventStartEndTimeDayWise>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventManagementId, opt => opt.Ignore());
            CreateMap<EventManagementTaskforces, EventManagementTaskforces>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventManagementId, opt => opt.Ignore());

            CreateMap<EventStaffDetail, EventStaffDetail>()
                .ForMember(dest => dest.AvailabilityDatesList, opt => opt.Ignore())
                .ForMember(dest => dest.EventWiseStaffRoleList, opt => opt.Ignore())
                .ForMember(dest => dest.EventWiseStaffSecondaryRoleList, opt => opt.Ignore());

            CreateMap<EventManagementStaffAvailability, EventManagementStaffAvailability>();
            CreateMap<EventWiseStaffRole, EventWiseStaffRole>();
            CreateMap<EventWiseStaffSecondaryRole, EventWiseStaffSecondaryRole>();

            // 🔹 EventStaff (Entity → Entity)
            CreateMap<EventStaff, EventStaff>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // preserve PK
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // do not override UserId
            .ForMember(dest => dest.StaffQualification, opt => opt.MapFrom(src => src.StaffQualification))
            .ForMember(dest => dest.StaffContractAffiliation, opt => opt.MapFrom(src => src.StaffContractAffiliation))
            .ForMember(dest => dest.TravelHonorList, opt => opt.MapFrom(src => src.TravelHonorList));

            CreateMap<StaffQualification, StaffQualification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventStaffId, opt => opt.Ignore());

            CreateMap<StaffContractAffiliation, StaffContractAffiliation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventStaffId, opt => opt.Ignore());

            CreateMap<TravelHonor, TravelHonor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EventStaffId, opt => opt.Ignore());

            CreateMap<StaffAttributeDetails, StaffAttributeDetails>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StaffQualificationId, opt => opt.Ignore());

            CreateMap<StaffLicenseDetails, StaffLicenseDetails>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StaffQualificationId, opt => opt.Ignore());

            // 🔹 ImmunizationVaccineInfo (Entity → Entity)
            CreateMap<ImmunizationVaccineInfo, ImmunizationVaccineInfo>()
                .ForMember(dest => dest.Lots, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            CreateMap<ImmunizationVaccineLotEntry, ImmunizationVaccineLotEntry>();

            //Post Event Management
            CreateMap<PostEventManagementDto, PostEventManagement>()
            .ForMember(dest => dest.PostEventStartEndTimeDayWise,
                opt => opt.MapFrom(src => src.PostEventStartEndTimeDayWiseDto))

            .ForMember(dest => dest.PostEventServiceDetails,
                opt => opt.MapFrom(src => src.EventServices))

            // Ignore navigation (handled by EF)
            .ForMember(dest => dest.EventManagement, opt => opt.Ignore())

            // Ignore properties not in entity
            .ForSourceMember(src => src.PostEventStatus, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.SubEventID, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventAddress1, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventAddress2, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventState, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventCity, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventZipCode, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.MOBDate, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.RegardingSites, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.Timezone, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.EventHelpLine, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.TaskForce, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.TotalServiceMember, opt => opt.DoNotValidate())
            .ForSourceMember(src => src.ContractDetails, opt => opt.DoNotValidate());

            // Child 1 Mapping
            CreateMap<PostEventStartEndTimeDayWiseDto, PostEventStartEndTimeDayWise>()
                .ForMember(dest => dest.PostEventManagementId, opt => opt.Ignore()) // set by EF
                .ForMember(dest => dest.PostEventManagement, opt => opt.Ignore());

            // Child 2 Mapping
            CreateMap<PostEventServiceDetailDto, PostEventServiceDetail>()
                .ForMember(dest => dest.PostEventManagementId, opt => opt.Ignore())
                .ForMember(dest => dest.EventServiceDetail, opt => opt.Ignore());

            //Post Event Lab Station
            CreateMap<PostEventLabStationDto, PostEventLabStation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Vital Station
            CreateMap<VitalStationBloodPressureReading, VitalStationBpReadingDto>();

            CreateMap<VitalStation, VitalStationDto>()
                .ForMember(dest => dest.PendingSystolic, opt => opt.Ignore())
                .ForMember(dest => dest.PendingDiastolic, opt => opt.Ignore())
                .ForMember(dest => dest.IsNextReadingRequired, opt => opt.Ignore())
                .ForMember(dest => dest.NextReadingNumber, opt => opt.Ignore())
                .ForMember(dest => dest.NextReadingAfterMinutes, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.NextBpReadingAvailableAt, opt => opt.Ignore())
                .ForMember(dest => dest.NextBpReadingUnlocked, opt => opt.Ignore())
                .ForMember(dest => dest.BloodPressureReadings,
                    opt => opt.MapFrom(src => (src.BloodPressureReadings ?? Array.Empty<VitalStationBloodPressureReading>())
                        .OrderBy(r => r.ReadingNumber)));

            CreateMap<VitalStationDto, VitalStation>()
                .ForMember(dest => dest.ServiceMembersChild, opt => opt.Ignore())
                .ForMember(dest => dest.BloodPressureReadings, opt => opt.Ignore())
                .ForMember(dest => dest.FinalBpStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TotalReadingsTaken, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            CreateMap<VitalStationBpReadingDto, VitalStationBloodPressureReading>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VitalStationId, opt => opt.Ignore())
                .ForMember(dest => dest.VitalStation, opt => opt.Ignore());
        }
    }
}
