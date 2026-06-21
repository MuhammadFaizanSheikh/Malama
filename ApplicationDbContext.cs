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
        //public DbSet<FileDataDto> FileData { get; set; }
        public DbSet<ServiceMembersParent> ServiceMembersParent { get; set; }
        public DbSet<ServiceMembersChild> ServiceMembersChild { get; set; }
        public DbSet<SubContractor> SubContractor { get; set; }
        public DbSet<SubmissionTokenRecord> SubmissionTokenRecord { get; set; }
        public DbSet<ContractDetails> ContractDetails { get; set; }
        public DbSet<EventStaff> EventStaff { get; set; }
        public DbSet<StaffQualification> StaffQualification { get; set; }
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
        public DbSet<UserEventMapping> UserEventMapping { get; set; }
        public DbSet<PostEventManagement> PostEventManagement { get; set; }
        public DbSet<PostEventLabStation> PostEventLabStation { get; set; }
        public DbSet<PostEventImmunizationStation> PostEventImmunizationStation { get; set; }
        public DbSet<VitalStation> VitalStation { get; set; }
        public DbSet<VitalStationBloodPressureReading> VitalStationBloodPressureReading { get; set; }
        public DbSet<DentalXRayStation> DentalXRayStation { get; set; }
        public DbSet<DentalXRayPaImage> DentalXRayPaImage { get; set; }
        public DbSet<DentalQuestionnaire> DentalQuestionnaire { get; set; }
        public DbSet<DentalExam> DentalExam { get; set; }
        public DbSet<DentalExamFinding> DentalExamFinding { get; set; }


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

            modelBuilder.Entity<ServiceMembersChild>()
            .HasIndex(x => new { x.ServiceMembersParentId, x.SmId })
            .IsUnique();

            modelBuilder.Entity<ImmunizationStation>(entity =>
            {
                entity.HasOne(e => e.HepBVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.HepBVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FluVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.FluVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MMRVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.MMRVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.HepAVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.HepAVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TetTdpVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.TetTdpVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.VaricellaVaccineInfo)
                    .WithMany()
                    .HasForeignKey(e => e.VaricellaVaccineInfoId)
                    .OnDelete(DeleteBehavior.Restrict);

                /* =========================================
                   ImmunizationStation → VaccineLotEntry
                   ========================================= */

                entity.HasOne(e => e.HepBVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.HepBVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FluVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.FluVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MMRVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.MMRVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.HepAVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.HepAVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TetTdpVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.TetTdpVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.VaricellaVaccineLot)
                    .WithMany()
                    .HasForeignKey(e => e.VaricellaVaccineLotEntryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DentalXRayStation>(entity =>
            {
                entity.HasOne(e => e.ServiceMembersChild)
                    .WithOne(s => s.DentalXRayStationRecord)
                    .HasForeignKey<DentalXRayStation>(e => e.ServiceMembersChildId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ServiceMembersChildId).IsUnique();
            });

            modelBuilder.Entity<DentalXRayPaImage>(entity =>
            {
                entity.HasOne(e => e.DentalXRayStation)
                    .WithMany(d => d.PaImages)
                    .HasForeignKey(e => e.DentalXRayStationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DentalQuestionnaire>(entity =>
            {
                entity.HasOne(e => e.ServiceMembersChild)
                    .WithOne(s => s.DentalQuestionnaireRecord)
                    .HasForeignKey<DentalQuestionnaire>(e => e.ServiceMembersChildId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ServiceMembersChildId).IsUnique();
            });

            modelBuilder.Entity<DentalExam>(entity =>
            {
                entity.HasOne(e => e.ServiceMembersChild)
                    .WithOne(s => s.DentalExamRecord)
                    .HasForeignKey<DentalExam>(e => e.ServiceMembersChildId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ServiceMembersChildId).IsUnique();
            });

            modelBuilder.Entity<DentalExamFinding>(entity =>
            {
                entity.HasOne(e => e.DentalExam)
                    .WithMany(d => d.Findings)
                    .HasForeignKey(e => e.DentalExamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }

}
