using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Malama.Models
{
    public class EventUsersViewModel
    {
        public long? SelectedEventId { get; set; }

        public List<EventViewModel> Events { get; set; } = new();

        public List<EventUserListDto> Users { get; set; } = new();
    }

    public class EventUserListDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PrimaryRole { get; set; }
        public string SecondaryRole { get; set; }
        public string PrimaryStation { get; set; }
        public string SecondaryStation { get; set; }
        public bool DetailSummaryAccess { get; set; }
        public string Attributes { get; set; }

        public List<UserPagePermissionDto> AllowedPages { get; set; } = new();
    }
}
