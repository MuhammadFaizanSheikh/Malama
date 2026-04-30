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
        public string PostEventStatus { get; set; }
        public DateTime EventStartDateUtc { get; set; }
        public DateTime EventEndDateUtc { get; set; }

        public string? PostEventNotes { get; set; }

        // ✅ Navigation (Parent)
        public virtual EventManagement EventManagement { get; set; }

        // ✅ One-to-Many (Child)
        public virtual IList<PostEventStartEndTimeDayWise> PostEventStartEndTimeDayWise { get; set; }
        public virtual IList<PostEventServiceDetail> PostEventServiceDetails { get; set; }
        public virtual IList<PostEventLabStation> PostEventLabStation { get; set; }
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

    [Table("PostEventServiceDetail")]
    public class PostEventServiceDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long PostEventManagementId { get; set; }
        public long EventServiceDetailId { get; set; }
        public int? PostEventNumbers { get; set; }

        public bool Completed { get; set; }

        // ✅ Navigation
        public virtual PostEventManagement PostEventManagement { get; set; }
        public virtual EventServiceDetail EventServiceDetail { get; set; }
    }
}
