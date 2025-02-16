using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExcelFilesCompiler
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your table as a DbSet
        public DbSet<FileDataDto> FileData { get; set; }
        public DbSet<SubContractor> SubContractor { get; set; }

        public DbSet<ContractDetails> ContractDetails { get; set; }
        public DbSet<EventStaff> EventStaff { get; set; }
        public DbSet<LicenseInfoDTO> EventStaffLicense { get; set; }
        public DbSet<StaffContractAffiliation> StaffContractAffiliation { get; set; }
        public DbSet<TravelHonor> TravelHonor { get; set; }
        public DbSet<EventManagement> EventManagement { get; set; }
        public DbSet<EventServiceDetail> EventServiceDetail { get; set; }
        public DbSet<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; set; }
        public DbSet<EventStaffDetail> EventStaffDetail { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
