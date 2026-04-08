namespace Malama.Models
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PostEventManagement")]
    public class PostEventManagement : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // FK to EventManagement
        public long EventManagementId { get; set; }

        public DateTime EventStartDateUtc { get; set; }
        public DateTime EventEndDateUtc { get; set; }

        public string PostEventNotes { get; set; }

        // ✅ Navigation (Parent)
        public virtual EventManagement EventManagement { get; set; }

        // ✅ One-to-Many (Child)
        public virtual ICollection<PostEventStartEndTimeDayWise> PostEventStartEndTimeDayWise { get; set; }
    }



    [Table("PostEventStartEndTimeDayWise")]
    public class PostEventStartEndTimeDayWise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long PostEventManagementId { get; set; }

        public int EventDay { get; set; }

        public TimeSpan? EventStartTime { get; set; }
        public TimeSpan? EventEndTime { get; set; }

        public int ServiceMemberPercentPerDay { get; set; }

        // ✅ Navigation
        public virtual PostEventManagement PostEventManagement { get; set; }
    }
}
