using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class TreatmentConsentHelper
    {
        public static TreatmentConsentListItemDto MapToListItem(
            ServiceMembersChild serviceMember,
            TreatmentConsent? consent = null)
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
                CompletedBy = consent?.UpdatedBy ?? consent?.AddedBy,
                CompletedOn = consent?.UpdatedOn ?? (consent != null ? consent.AddedOn : null),
                Status = ComputeStationStatus(consent)
            };
        }

        public static List<TreatmentConsentListItemDto> MapToListItems(
            IEnumerable<ServiceMembersChild> serviceMembers,
            IReadOnlyDictionary<long, TreatmentConsent>? consentsBySmId = null)
        {
            var lookup = consentsBySmId ?? new Dictionary<long, TreatmentConsent>();
            return (serviceMembers ?? Enumerable.Empty<ServiceMembersChild>())
                .Select(sm =>
                {
                    lookup.TryGetValue(sm.Id, out var consent);
                    return MapToListItem(sm, consent);
                })
                .ToList();
        }

        public static TreatmentConsentIndexViewModel BuildIndexViewModel(
            IEnumerable<ServiceMembersChild> serviceMembers,
            string? eventId,
            IReadOnlyDictionary<long, TreatmentConsent>? consentsBySmId = null)
        {
            var items = MapToListItems(serviceMembers, consentsBySmId);
            var completedCount = items.Count(x => IsCompleted(x.Status));
            return new TreatmentConsentIndexViewModel
            {
                EventId = eventId,
                TotalCount = items.Count,
                PendingCount = items.Count - completedCount,
                CompletedCount = completedCount,
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

        public static string ComputeStationStatus(TreatmentConsent? entity)
        {
            if (entity == null)
            {
                return AppConstants.Status.Pending;
            }

            return ComputeStationStatus(
                entity.IncludeOralSurgeryForm,
                ParseIds(entity.OralSurgeryDentistEventStaffIdsJson),
                ParseOralForms(entity.OralSurgeryFormsJson),
                entity.IncludeDentalTreatmentConsent,
                ParseIds(entity.DentalTreatmentDentistEventStaffIdsJson),
                ParseTreatmentForms(entity.DentalTreatmentFormsJson));
        }

        public static string ComputeStationStatus(TreatmentConsentFormSelectionDto? selection)
        {
            if (selection == null)
            {
                return AppConstants.Status.Pending;
            }

            return ComputeStationStatus(
                selection.IncludeOralSurgeryForm,
                selection.OralSurgeryDentistEventStaffIds,
                selection.OralSurgeryForms,
                selection.IncludeDentalTreatmentConsent,
                selection.DentalTreatmentDentistEventStaffIds,
                selection.DentalTreatmentForms);
        }

        public static string ComputeStationStatus(
            bool includeOralSurgeryForm,
            IEnumerable<long>? oralSurgeryDentistIds,
            IEnumerable<TreatmentConsentOralSurgeryFormDto>? oralSurgeryForms,
            bool includeDentalTreatmentConsent,
            IEnumerable<long>? dentalTreatmentDentistIds,
            IEnumerable<TreatmentConsentDentalTreatmentFormDto>? dentalTreatmentForms)
        {
            var oralIds = includeOralSurgeryForm
                ? NormalizeIds(oralSurgeryDentistIds)
                : new List<long>();
            var treatmentIds = includeDentalTreatmentConsent
                ? NormalizeIds(dentalTreatmentDentistIds)
                : new List<long>();

            if (oralIds.Count == 0 && treatmentIds.Count == 0)
            {
                return AppConstants.Status.Pending;
            }

            if (includeOralSurgeryForm)
            {
                if (oralIds.Count == 0)
                {
                    return AppConstants.Status.Pending;
                }

                var oralById = (oralSurgeryForms ?? Enumerable.Empty<TreatmentConsentOralSurgeryFormDto>())
                    .Where(form => form != null && form.EventStaffId > 0)
                    .GroupBy(form => form.EventStaffId)
                    .ToDictionary(group => group.Key, group => group.Last());

                foreach (var id in oralIds)
                {
                    if (!oralById.TryGetValue(id, out var form) || !IsFormSigned(form.IsSigned, form.SignatureFileName))
                    {
                        return AppConstants.Status.Pending;
                    }
                }
            }

            if (includeDentalTreatmentConsent)
            {
                if (treatmentIds.Count == 0)
                {
                    return AppConstants.Status.Pending;
                }

                var treatmentById = (dentalTreatmentForms ?? Enumerable.Empty<TreatmentConsentDentalTreatmentFormDto>())
                    .Where(form => form != null && form.EventStaffId > 0)
                    .GroupBy(form => form.EventStaffId)
                    .ToDictionary(group => group.Key, group => group.Last());

                foreach (var id in treatmentIds)
                {
                    if (!treatmentById.TryGetValue(id, out var form) || !IsFormSigned(form.IsSigned, form.SignatureFileName))
                    {
                        return AppConstants.Status.Pending;
                    }
                }
            }

            return AppConstants.Status.Completed;
        }

        public static bool IsFormSigned(bool isSigned, string? signatureFileName)
        {
            return isSigned && !string.IsNullOrWhiteSpace(signatureFileName);
        }

        /// <summary>
        /// Builds consent-form status rows for the Treatment Coordinator card.
        /// When no per-dentist forms exist, returns an empty list (UI shows an empty-state message).
        /// </summary>
        public static List<TreatmentCoordinatorConsentFormStatusItem> BuildCoordinatorConsentFormStatusItems(
            TreatmentConsentFormSelectionDto? selection)
        {
            var items = new List<TreatmentCoordinatorConsentFormStatusItem>();
            if (selection == null)
            {
                return items;
            }

            if (selection.IncludeDentalTreatmentConsent)
            {
                var formsById = (selection.DentalTreatmentForms ?? new List<TreatmentConsentDentalTreatmentFormDto>())
                    .Where(form => form != null && form.EventStaffId > 0)
                    .GroupBy(form => form.EventStaffId)
                    .ToDictionary(group => group.Key, group => group.Last());

                foreach (var dentistId in NormalizeIds(selection.DentalTreatmentDentistEventStaffIds))
                {
                    formsById.TryGetValue(dentistId, out var form);
                    var dentistName = form?.DentistName?.Trim();
                    if (string.IsNullOrWhiteSpace(dentistName))
                    {
                        dentistName = "Dentist #" + dentistId;
                    }

                    items.Add(new TreatmentCoordinatorConsentFormStatusItem
                    {
                        Title = "Dental Treatment Consent Form for " + dentistName,
                        IsSigned = form != null && IsFormSigned(form.IsSigned, form.SignatureFileName),
                        FormKind = "dental-treatment",
                        EventStaffId = dentistId
                    });
                }
            }

            if (selection.IncludeOralSurgeryForm)
            {
                var formsById = (selection.OralSurgeryForms ?? new List<TreatmentConsentOralSurgeryFormDto>())
                    .Where(form => form != null && form.EventStaffId > 0)
                    .GroupBy(form => form.EventStaffId)
                    .ToDictionary(group => group.Key, group => group.Last());

                foreach (var dentistId in NormalizeIds(selection.OralSurgeryDentistEventStaffIds))
                {
                    formsById.TryGetValue(dentistId, out var form);
                    var dentistName = form?.DentistName?.Trim();
                    if (string.IsNullOrWhiteSpace(dentistName))
                    {
                        dentistName = "Dentist #" + dentistId;
                    }

                    items.Add(new TreatmentCoordinatorConsentFormStatusItem
                    {
                        Title = "Oral Surgery Form for " + dentistName,
                        IsSigned = form != null && IsFormSigned(form.IsSigned, form.SignatureFileName),
                        FormKind = "oral-surgery",
                        EventStaffId = dentistId
                    });
                }
            }

            return items;
        }

        public static bool IsCompleted(string? status)
        {
            return string.Equals(status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase);
        }

        private static List<long> NormalizeIds(IEnumerable<long>? ids)
        {
            return (ids ?? Enumerable.Empty<long>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();
        }

        private static List<TreatmentConsentOralSurgeryFormDto> ParseOralForms(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentConsentOralSurgeryFormDto>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TreatmentConsentOralSurgeryFormDto>>(json)
                    ?? new List<TreatmentConsentOralSurgeryFormDto>();
            }
            catch
            {
                return new List<TreatmentConsentOralSurgeryFormDto>();
            }
        }

        private static List<TreatmentConsentDentalTreatmentFormDto> ParseTreatmentForms(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentConsentDentalTreatmentFormDto>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TreatmentConsentDentalTreatmentFormDto>>(json)
                    ?? new List<TreatmentConsentDentalTreatmentFormDto>();
            }
            catch
            {
                return new List<TreatmentConsentDentalTreatmentFormDto>();
            }
        }

        private static List<long> ParseIds(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<long>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<long>>(json)?
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList()
                    ?? new List<long>();
            }
            catch
            {
                return new List<long>();
            }
        }
    }
}
