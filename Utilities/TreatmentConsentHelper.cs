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

        public const string OralSurgeryDiseaseType = "Oral Surgery";

        /// <summary>
        /// Distinct appointment dentists bucketed by linked finding disease type.
        /// Oral Surgery → oral list; any other non-empty disease type → dental treatment list.
        /// </summary>
        public static TreatmentConsentDerivedDentists DeriveDentistsFromAppointments(
            IEnumerable<DentalAppointment>? appointments,
            IEnumerable<DentalFinding>? findings)
        {
            var findingByKey = (findings ?? Enumerable.Empty<DentalFinding>())
                .Where(f => !string.IsNullOrWhiteSpace(f.ClientKey))
                .GroupBy(f => f.ClientKey!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);

            var oralIds = new HashSet<long>();
            var treatmentIds = new HashSet<long>();

            foreach (var appointment in appointments ?? Enumerable.Empty<DentalAppointment>())
            {
                if (appointment == null || appointment.EventStaffId <= 0)
                {
                    continue;
                }

                foreach (var link in appointment.Findings ?? Enumerable.Empty<DentalAppointmentFinding>())
                {
                    var key = link?.FindingClientKey?.Trim();
                    if (string.IsNullOrWhiteSpace(key)
                        || !findingByKey.TryGetValue(key, out var finding))
                    {
                        continue;
                    }

                    var diseaseType = finding.DiseaseConditionType?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(diseaseType))
                    {
                        continue;
                    }

                    if (string.Equals(diseaseType, OralSurgeryDiseaseType, StringComparison.OrdinalIgnoreCase))
                    {
                        oralIds.Add(appointment.EventStaffId);
                    }
                    else
                    {
                        treatmentIds.Add(appointment.EventStaffId);
                    }
                }
            }

            return new TreatmentConsentDerivedDentists
            {
                OralSurgeryDentistEventStaffIds = oralIds.OrderBy(id => id).ToList(),
                DentalTreatmentDentistEventStaffIds = treatmentIds.OrderBy(id => id).ToList()
            };
        }

        public static void ApplyDerivedDentistsToSelection(
            TreatmentConsentFormSelectionDto selection,
            TreatmentConsentDerivedDentists derived,
            IReadOnlyDictionary<long, string>? dentistNamesById = null)
        {
            if (selection == null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            derived ??= new TreatmentConsentDerivedDentists();
            var names = dentistNamesById ?? new Dictionary<long, string>();

            selection.OralSurgeryDentistEventStaffIds = NormalizeIds(derived.OralSurgeryDentistEventStaffIds);
            selection.DentalTreatmentDentistEventStaffIds = NormalizeIds(derived.DentalTreatmentDentistEventStaffIds);
            selection.IncludeOralSurgeryForm = selection.OralSurgeryDentistEventStaffIds.Count > 0;
            selection.IncludeDentalTreatmentConsent = selection.DentalTreatmentDentistEventStaffIds.Count > 0;

            selection.OralSurgeryForms = MergeOralForms(
                selection.OralSurgeryForms,
                selection.OralSurgeryDentistEventStaffIds,
                names);
            selection.DentalTreatmentForms = MergeTreatmentForms(
                selection.DentalTreatmentForms,
                selection.DentalTreatmentDentistEventStaffIds,
                names);

            selection.Status = ComputeStationStatus(selection);
        }

        private static List<TreatmentConsentOralSurgeryFormDto> MergeOralForms(
            IEnumerable<TreatmentConsentOralSurgeryFormDto>? existingForms,
            IEnumerable<long> dentistIds,
            IReadOnlyDictionary<long, string> dentistNamesById)
        {
            var byId = (existingForms ?? Enumerable.Empty<TreatmentConsentOralSurgeryFormDto>())
                .Where(f => f != null && f.EventStaffId > 0)
                .GroupBy(f => f.EventStaffId)
                .ToDictionary(g => g.Key, g => g.Last());

            var result = new List<TreatmentConsentOralSurgeryFormDto>();
            foreach (var id in NormalizeIds(dentistIds))
            {
                if (!byId.TryGetValue(id, out var form) || form == null)
                {
                    form = new TreatmentConsentOralSurgeryFormDto { EventStaffId = id };
                }

                if (string.IsNullOrWhiteSpace(form.DentistName)
                    && dentistNamesById.TryGetValue(id, out var name)
                    && !string.IsNullOrWhiteSpace(name))
                {
                    form.DentistName = name.Trim();
                }

                result.Add(form);
            }

            return result;
        }

        private static List<TreatmentConsentDentalTreatmentFormDto> MergeTreatmentForms(
            IEnumerable<TreatmentConsentDentalTreatmentFormDto>? existingForms,
            IEnumerable<long> dentistIds,
            IReadOnlyDictionary<long, string> dentistNamesById)
        {
            var byId = (existingForms ?? Enumerable.Empty<TreatmentConsentDentalTreatmentFormDto>())
                .Where(f => f != null && f.EventStaffId > 0)
                .GroupBy(f => f.EventStaffId)
                .ToDictionary(g => g.Key, g => g.Last());

            var result = new List<TreatmentConsentDentalTreatmentFormDto>();
            foreach (var id in NormalizeIds(dentistIds))
            {
                if (!byId.TryGetValue(id, out var form) || form == null)
                {
                    form = new TreatmentConsentDentalTreatmentFormDto { EventStaffId = id };
                }

                if (string.IsNullOrWhiteSpace(form.DentistName)
                    && dentistNamesById.TryGetValue(id, out var name)
                    && !string.IsNullOrWhiteSpace(name))
                {
                    form.DentistName = name.Trim();
                }

                result.Add(form);
            }

            return result;
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

    public class TreatmentConsentDerivedDentists
    {
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
    }
}
