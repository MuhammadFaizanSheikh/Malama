namespace Malama.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PostEventManagementViewModel
    {
        public List<PostEventManagementPreview>? EventManagements { get; set; }
    }

    public class PostEventManagementPreview
    {
        public long Id { get; set; }
        public long? PostEventManagementId { get; set; }
        public string? EventID { get; set; }
        public int EventVersion { get; set; }
        public string? EventStatus { get; set; }
        public string? TaskForce { get; set; }
        public string? EventState { get; set; }
        public string? EventCity { get; set; }
        public string? EventZipCode { get; set; }
        public string? StatusDescription { get; set; }
        public DateTime EventStartDateUtc { get; set; }
        public DateTime EventEndDateUtc { get; set; }
        public Boolean CanEdit { get; set; }
        
    }

    public class PostEventManagementDto
    {
        public long Id { get; set; }
        public long EventManagementId { get; set; }

        public DateTime EventStartDateUtc { get; set; }
        public DateTime EventEndDateUtc { get; set; }

        public string? PostEventNotes { get; set; }

        public string PostEventStatus { get; set; }
        public string? EventID { get; set; }
        public string? SubEventID { get; set; }
        public string? EventAddress1 { get; set; }
        public string? EventAddress2 { get; set; }
        public string? EventState { get; set; }
        public string? EventCity { get; set; }
        public string? EventZipCode { get; set; }
        public DateTime? MOBDate { get; set; }
        public string? RegardingSites { get; set; }
        public string? Timezone { get; set; }
        public string? EventHelpLine { get; set; }
        public string? TaskForce { get; set; }

        [Range(0, 99999, ErrorMessage = "Value must be between 0 and 99999")]
        public long TotalServiceMember { get; set; }

        [ValidateNever]
        public ContractDetails? ContractDetails { get; set; }
        public List<PostEventStartEndTimeDayWiseDto> PostEventStartEndTimeDayWiseDto { get; set; }

        public List<PostEventServiceDetailDto> EventServices { get; set; }

        public string SubmissionToken { get; set; }
    }

    public class PostEventStartEndTimeDayWiseDto
    {
        public long Id { get; set; }

        public int EventDay { get; set; }

        public TimeSpan? EventStartTime { get; set; }
        public TimeSpan? EventEndTime { get; set; }

        public int ServiceMemberPercentPerDay { get; set; }
    }

    public class PostEventServiceDetailDto
    {
        public long Id { get; set; }
        public long EventServiceDetailId { get; set; }

        public string EventService { get; set; }

        public int? FinalPreEventConfirmedNumbers { get; set; }

        public int? PostEventNumbers { get; set; }

        public bool Completed { get; set; }
    }
}
