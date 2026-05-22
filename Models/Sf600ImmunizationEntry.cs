namespace Malama.Models
{
    public class Sf600ImmunizationEntry
    {
        public string VaccineTitle { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? Dose { get; set; }
        public string? Unit { get; set; }
        public string? LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? AdministrationType { get; set; }
        public string? BodyPart { get; set; }
        public string? Site { get; set; }
        public string? StaffName { get; set; }
        public DateTime? GivenDateTime { get; set; }
    }
}
