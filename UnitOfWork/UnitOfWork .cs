using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepository<SubmissionTokenRecord> SubmissionTokenRecord { get; private set; }
        public IGenericRepository<ContractDetails> ContractDetails { get; private set; }
        public IGenericRepository<SubContractor> SubContractors { get; private set; }
        public IGenericRepository<EventStaff> EventStaff { get; private set; }
        public IGenericRepository<StaffQualification> StaffQualification { get; private set; }
        public IGenericRepository<StaffContractAffiliation> StaffContractAffiliation { get; private set; }
        public IGenericRepository<TravelHonor> TravelHonor { get; private set; }
        public IGenericRepository<ServiceTypeProvided> ServiceTypeProvided { get; private set; }
        public IGenericRepository<EventManagement> EventManagement { get; private set; }
        public IGenericRepository<EventServiceDetail> EventServiceDetail { get; private set; }
        public IGenericRepository<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; private set; }
        public IGenericRepository<EventStaffDetail> EventStaffDetail { get; private set; }
        public IGenericRepository<EventManagementTaskforces> EventManagementTaskforces { get; private set; }
        public IGenericRepository<EventManagementStaffAvailability> EventManagementStaffAvailability { get; private set; }
        public IGenericRepository<ImmunizationStation> ImmunizationStation { get; private set; }
        public IGenericRepository<ImmunizationVaccineInfo> ImmunizationVaccineInfo { get; private set; }
        public IGenericRepository<ImmunizationVaccineLotEntry> ImmunizationVaccineLotEntry { get; private set; }
        public IGenericRepository<ContainerType> ContainerType { get; private set; }
        public IGenericRepository<Container> Container { get; private set; }
        public IGenericRepository<ContainerTemperatureReading> ContainerTemperatureReading { get; private set; }
        public IGenericRepository<ContainerNotification> ContainerNotification { get; private set; }
        public IGenericRepository<UserEventMapping> UserEventMapping { get; private set; }
        public IGenericRepository<LabStation> LabStation { get; private set; }
        public IGenericRepository<ServiceMembersParent> ServiceMembersParent { get; private set; }
        public IGenericRepository<ServiceMembersChild> ServiceMembersChild { get; private set; }
        public IGenericRepository<PostEventManagement> PostEventManagement { get; private set; }
        public IGenericRepository<PostEventLabStation> PostEventLabStation { get; private set; }
        public IGenericRepository<PostEventImmunizationStation> PostEventImmunizationStation { get; private set; }
        public IGenericRepository<VitalStation> VitalStation { get; private set; }
        public IGenericRepository<DentalXRayStation> DentalXRayStation { get; private set; }
        public IGenericRepository<DentalXRayPaImage> DentalXRayPaImage { get; private set; }
        public IGenericRepository<DentalQuestionnaire> DentalQuestionnaire { get; private set; }
        public IGenericRepository<DentalExam> DentalExam { get; private set; }
        public IGenericRepository<DentalExamFinding> DentalExamFinding { get; private set; }
        public IGenericRepository<DentalExamSelectedTooth> DentalExamSelectedTooth { get; private set; }
        public IGenericRepository<DentalTreatment> DentalTreatment { get; private set; }
        public IGenericRepository<DentalTreatmentFinding> DentalTreatmentFinding { get; private set; }
        public IGenericRepository<DentalTreatmentSelectedTooth> DentalTreatmentSelectedTooth { get; private set; }
        public IGenericRepository<DentalTreatmentAnesthesia> DentalTreatmentAnesthesia { get; private set; }
        public IGenericRepository<DentalTreatmentPrescription> DentalTreatmentPrescription { get; private set; }
        public IGenericRepository<DentalTreatmentOverallNote> DentalTreatmentOverallNote { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            SubmissionTokenRecord = new GenericRepository<SubmissionTokenRecord>(_context);
            ContractDetails = new GenericRepository<ContractDetails>(_context);
            SubContractors = new GenericRepository<SubContractor>(_context);
            EventStaff = new GenericRepository<EventStaff>(_context);
            StaffQualification = new GenericRepository<StaffQualification>(_context);
            StaffContractAffiliation = new GenericRepository<StaffContractAffiliation>(_context);
            TravelHonor = new GenericRepository<TravelHonor>(_context);
            ServiceTypeProvided = new GenericRepository<ServiceTypeProvided>(_context);
            EventManagement = new GenericRepository<EventManagement>(_context);
            EventServiceDetail = new GenericRepository<EventServiceDetail>(_context);
            EventStartEndTimeDayWise = new GenericRepository<EventStartEndTimeDayWise>(_context);
            EventStaffDetail = new GenericRepository<EventStaffDetail>(_context);
            EventManagementTaskforces = new GenericRepository<EventManagementTaskforces>(_context);
            EventManagementStaffAvailability = new GenericRepository<EventManagementStaffAvailability>(_context);
            ImmunizationStation = new GenericRepository<ImmunizationStation>(_context);
            ImmunizationVaccineInfo = new GenericRepository<ImmunizationVaccineInfo>(_context);
            ImmunizationVaccineLotEntry = new GenericRepository<ImmunizationVaccineLotEntry>(_context);
            ContainerType = new GenericRepository<ContainerType>(_context);
            Container = new GenericRepository<Container>(_context);
            ContainerTemperatureReading = new GenericRepository<ContainerTemperatureReading>(_context);
            ContainerNotification = new GenericRepository<ContainerNotification>(_context);
            UserEventMapping = new GenericRepository<UserEventMapping>(_context);
            LabStation = new GenericRepository<LabStation>(_context);
            ServiceMembersParent = new GenericRepository<ServiceMembersParent>(_context);
            ServiceMembersChild = new GenericRepository<ServiceMembersChild>(_context);
            PostEventManagement = new GenericRepository<PostEventManagement>(_context);
            PostEventLabStation = new GenericRepository<PostEventLabStation>(_context);
            PostEventImmunizationStation = new GenericRepository<PostEventImmunizationStation>(_context);
            VitalStation = new GenericRepository<VitalStation>(_context);
            DentalXRayStation = new GenericRepository<DentalXRayStation>(_context);
            DentalXRayPaImage = new GenericRepository<DentalXRayPaImage>(_context);
            DentalQuestionnaire = new GenericRepository<DentalQuestionnaire>(_context);
            DentalExam = new GenericRepository<DentalExam>(_context);
            DentalExamFinding = new GenericRepository<DentalExamFinding>(_context);
            DentalExamSelectedTooth = new GenericRepository<DentalExamSelectedTooth>(_context);
            DentalTreatment = new GenericRepository<DentalTreatment>(_context);
            DentalTreatmentFinding = new GenericRepository<DentalTreatmentFinding>(_context);
            DentalTreatmentSelectedTooth = new GenericRepository<DentalTreatmentSelectedTooth>(_context);
            DentalTreatmentAnesthesia = new GenericRepository<DentalTreatmentAnesthesia>(_context);
            DentalTreatmentPrescription = new GenericRepository<DentalTreatmentPrescription>(_context);
            DentalTreatmentOverallNote = new GenericRepository<DentalTreatmentOverallNote>(_context);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SetValues<TEntity>(TEntity existing, TEntity updated) where TEntity : class
        {
            _context.Entry(existing).CurrentValues.SetValues(updated);
        }
    }
}
