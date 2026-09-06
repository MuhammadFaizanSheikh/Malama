using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class TreatmentConsentHelper
    {
        public static TreatmentConsentListItemDto MapToListItem(ServiceMembersChild serviceMember)
        {
            if (serviceMember == null)
            {
                throw new ArgumentNullException(nameof(serviceMember));
            }

            return new TreatmentConsentListItemDto
            {
                ServiceMembersChildId = serviceMember.Id,
                SmId = serviceMember.SmId,
                Drc = serviceMember.Drc,
                FullName = serviceMember.FullName,
                Last4 = serviceMember.Last4,
                DodId = serviceMember.DodId,
                Sex = serviceMember.Sex,
                Dob = serviceMember.Dob,
                Barcode = serviceMember.Barcode,
                CheckInBy = serviceMember.CheckInBy,
                CheckInTime = serviceMember.CheckInTime,
                Status = AppConstants.Status.Pending
            };
        }

        public static List<TreatmentConsentListItemDto> MapToListItems(IEnumerable<ServiceMembersChild> serviceMembers)
        {
            return (serviceMembers ?? Enumerable.Empty<ServiceMembersChild>())
                .Select(MapToListItem)
                .ToList();
        }

        public static TreatmentConsentIndexViewModel BuildIndexViewModel(
            IEnumerable<ServiceMembersChild> serviceMembers,
            string? eventId)
        {
            var items = MapToListItems(serviceMembers);
            return new TreatmentConsentIndexViewModel
            {
                EventId = eventId,
                TotalCount = items.Count,
                ServiceMembers = items
            };
        }

        public static bool IsCheckedIn(ServiceMembersChild? serviceMember)
        {
            if (serviceMember == null)
            {
                return false;
            }

            return string.Equals(
                serviceMember.CheckIn,
                AppConstants.YesNo.Yes,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
