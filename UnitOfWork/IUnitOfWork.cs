using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ContractDetails> ContractDetails { get; }
        IGenericRepository<SubContractor> SubContractors { get; }
        IGenericRepository<EventStaff> EventStaff { get; }
        IGenericRepository<StaffQualification> StaffQualification { get; }
        IGenericRepository<StaffContractAffiliation> StaffContractAffiliation { get; }
        IGenericRepository<TravelHonor> TravelHonor { get; }
        IGenericRepository<ServiceTypeProvided> ServiceTypeProvided { get; }
        IGenericRepository<EventManagement> EventManagement { get; }
        IGenericRepository<EventServiceDetail> EventServiceDetail { get; }
        IGenericRepository<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; }
        IGenericRepository<EventStaffDetail> EventStaffDetail { get; }
        IGenericRepository<EventManagementTaskforces> EventManagementTaskforces { get; }
        IGenericRepository<EventManagementStaffAvailability> EventManagementStaffAvailability { get; }
        IGenericRepository<ImmunizationStation> ImmunizationStation { get; }
        IGenericRepository<ImmunizationVaccineInfo> ImmunizationVaccineInfo { get; }
        IGenericRepository<ImmunizationVaccineLotEntry> ImmunizationVaccineLotEntry { get; }
        IGenericRepository<ContainerType> ContainerType { get; }
        IGenericRepository<Container> Container { get; }
        IGenericRepository<ContainerTemperatureReading> ContainerTemperatureReading { get; }
        IGenericRepository<ContainerNotification> ContainerNotification { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveAsync();
    }
}
