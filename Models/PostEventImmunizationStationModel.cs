namespace Malama.Models
{
    public class PostEventImmunizationStation : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }
        public ServiceMembersChild ServiceMembersChild { get; set; }

        public long PostEventManagementId { get; set; }
        public PostEventManagement PostEventManagement { get; set; }

        public string Status { get; set; } = "Pending";

        public bool HepBDataEntered { get; set; }
        public DateTime? HepBDataEnteredDateTime { get; set; }
        public bool HepADataEntered { get; set; }
        public DateTime? HepADataEnteredDateTime { get; set; }
        public bool FluDataEntered { get; set; }
        public DateTime? FluDataEnteredDateTime { get; set; }
        public bool MmrDataEntered { get; set; }
        public DateTime? MmrDataEnteredDateTime { get; set; }
        public bool TetTdpDataEntered { get; set; }
        public DateTime? TetTdpDataEnteredDateTime { get; set; }
        public bool VaricellaDataEntered { get; set; }
        public DateTime? VaricellaDataEnteredDateTime { get; set; }
    }
}
