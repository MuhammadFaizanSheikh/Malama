using ExcelFilesCompiler.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExcelToCsv.Models
{
    [Index(nameof(EventId), nameof(isDeleted))]
    public class FileDataDto : GenericProperties
    {
        [Key]
        public long Id { get; set; }
        public long? SmId { get; set; }
        public string? FullName { get; set; }
        //public string? Name { get; set; }
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
        public string? PregnancyTestNeeded { get; set; }
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
        public string? CheckIn { get; set; } = "No";
        public string? CheckInBy { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string? CheckOut { get; set; } = "No";
        public string? CheckOutBy { get; set; } = "No";
        public DateTime? CheckOutTime { get; set; }
        public string? WalkInServiceMember { get; set; } = "No";
        public int? VisionWin { get; set; }
        public int? DentalWin { get; set; }
        public int? PhaWin { get; set; }
        public int? HivWin { get; set; }
        public int? HearingWin { get; set; }
        public bool? isDeleted { get; set; }
        public string? Barcode { get; set; }

        //Check Out Section Fields
        public string? VitalsStatus { get; set; }
        public string? HepBNeededStatus { get; set; }
        public string? HepBReason { get; set; }
        public string? FluNeededStatus { get; set; }
        public string? FluReason { get; set; }
        public string? MmrStatus { get; set; }
        public string? MmrReason { get; set; }
        public string? HepANeededStatus { get; set; }
        public string? HepAReason { get; set; }
        public string? TetTdpNeededStatus { get; set; }
        public string? TetTdpReason { get; set; }
        public string? VaricellaNeededStatus { get; set; }
        public string? VaricellaReason { get; set; }

        public string? SickleStatus { get; set; }
        public string? SickleReason { get; set; }
        public string? HivStatus { get; set; }
        public string? HivBarcode { get; set; }
        public string? HivReason { get; set; }
        public string? AboStatus { get; set; }
        public string? AboReason { get; set; }
        public string? DnaStatus { get; set; }
        public string? DnaReason { get; set; }
        public string? G6pdCheckoutStatus { get; set; }
        public string? G6pdReason { get; set; }
        public string? LipidStatus { get; set; }
        public string? LipidReason { get; set; }
        public string? EkgNeededStatus { get; set; }
        public string? EkgNeededReason { get; set; }

        public string? PhaStatus { get; set; }
        public string? PhaFollowUp { get; set; }
        public string? HearingStatus { get; set; }
        public string? AudiologistServiceCompleted { get; set; }
        public string? VisionStatus { get; set; }
        public string? OptometristServiceCompleted { get; set; }

        public string? DentalXrayStatus { get; set; }
        public string? PanoramicXray { get; set; }
        public string? DentalExamStatus { get; set; }
        public string? ClassDentalExam { get; set; }
        public string? DentalTreatment { get; set; }
        public string? FinalDentalClass { get; set; }
        public string? DentalTreatmentClassReason { get; set; }
        public string? DentalTreatmentReason { get; set; }
        

    }
}
