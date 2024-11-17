using ExcelFilesCompiler.Models;
using System.ComponentModel.DataAnnotations;

namespace ExcelToCsv.Models
{
    public class FileDataDto : GenericProperties
    {
        [Key]
        public long? Id { get; set; }
        public long? SmId { get; set; }
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? FullSsn { get; set; }
        public string? Last4 { get; set; }
        public string? DodId { get; set; }
        public string? Rank { get; set; }
        public int? Age { get; set; }
        public string? Sex { get; set; }
        public string? Mos { get; set; }
        public string? Agr { get; set; }
        public string? Uic { get; set; }
        public string? Mrc { get; set; }
        public string? Dob { get; set; }
        public string? Over40 { get; set; }
        public string? DentalDue { get; set; }
        public string? DentalExam { get; set; }
        public string? DentalNeeded { get; set; }
        public string? PanoNeeded { get; set; }
        public string? BwxNeeded { get; set; }
        public string? Drc { get; set; }
        public string? PhaDate { get; set; }
        public string? PhaDue { get; set; }
        public string? Pha { get; set; }
        public string? Pulhes { get; set; }
        public string? VisionDate { get; set; }
        public string? Vision { get; set; }
        public string? NearVision { get; set; }
        public string? Vrc { get; set; }
        public string? Vision2pg { get; set; }
        public string? Vision1mi { get; set; }
        public string? HearingDate { get; set; }
        public string? Hearing { get; set; }
        public string? Hrc { get; set; }
        public string? HearingProfile { get; set; }
        public string? Quest { get; set; }
        public string? LabNeeded { get; set; }
        public string? Abo { get; set; }
        public string? AboNeeded { get; set; }
        public string? Dna { get; set; }
        public string? SickleDate { get; set; }
        public string? Sickle { get; set; }
        public string? G6pd { get; set; }
        public string? G6pdDate { get; set; }
        public string? G6pdStatus { get; set; }
        public string? HivNextTestDate { get; set; }
        public string? Hiv { get; set; }
        public string? LipidNeeded { get; set; }
        public string? LipidPanel { get; set; }
        public string? CholesterolHdlCholesterol { get; set; }
        public string? Framingham { get; set; }
        public string? Ekg { get; set; }
        public string? EkgNeeded { get; set; }
        public string? Hcg { get; set; }
        public string? Imm { get; set; }
        public string? HepB { get; set; }
        public string? HepA { get; set; }
        public string? Flu { get; set; }
        public string? TetTdp { get; set; }
        public string? Mmr { get; set; }
        public string? Varicella { get; set; }
        public string? TaskForce { get; set; }
        public string? Notes { get; set; }
        public string? Over44 { get; set; }
        public string? EventDate { get; set; }
        public string? EventEndDate { get; set; }
        public string? EventId { get; set; }
        public string? CheckIn { get; set; } = "0";
        public DateTime? CheckInDateTime { get; set; }
        public string? CheckOut { get; set; } = "0";
        public DateTime? CheckOutDateTime { get; set; }
        public int? VisionWin { get; set; }
        public int? DentalWin { get; set; }
        public int? PhaWin { get; set; }
        public int? HivWin { get; set; }
        public int? HearingWin { get; set; }
        public bool? isDeleted { get; set; }
    }
}
