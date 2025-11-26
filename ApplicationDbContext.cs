using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExcelFilesCompiler
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your table as a DbSet
        public DbSet<FileDataDto> FileData { get; set; }
        public DbSet<SubContractor> SubContractor { get; set; }

        public DbSet<ContractDetails> ContractDetails { get; set; }
        public DbSet<EventStaff> EventStaff { get; set; }
        public DbSet<StaffLicense> StaffLicense { get; set; }
        public DbSet<StaffContractAffiliation> StaffContractAffiliation { get; set; }
        public DbSet<TravelHonor> TravelHonor { get; set; }
        public DbSet<EventManagement> EventManagement { get; set; }
        public DbSet<EventServiceDetail> EventServiceDetail { get; set; }
        public DbSet<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; set; }
        public DbSet<EventStaffDetail> EventStaffDetail { get; set; }
        public DbSet<ImmunizationStation> ImmunizationStation { get; set; }
        public DbSet<ImmunizationVaccineInfo> ImmunizationVaccineInfo { get; set; }
        public DbSet<ContainerType> ContainerType { get; set; }
        public DbSet<Container> Container { get; set; }
        public DbSet<ContainerTemperatureReading> ContainerTemperatureReading { get; set; }
        public DbSet<ContainerNotification> ContainerNotification { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global override for PostgreSQL only
            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        {
                            property.SetColumnType("timestamp without time zone");
                        }
                    }
                }
            }
        }

    }

}
