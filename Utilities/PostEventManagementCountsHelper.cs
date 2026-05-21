using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    /// <summary>
    /// Computes PostEventNumbers from already-loaded lab and immunization station records.
    /// </summary>
    public static class PostEventManagementCountsHelper
    {
        private static readonly string[] LabFinishedStatuses =
        {
            AppConstants.LabResultStatus.Complete,
            AppConstants.LabResultStatus.CompleteWithReason,
            "Completed"
        };

        public static void ApplyPostEventNumbers(
            IList<PostEventServiceDetailDto>? eventServices,
            IReadOnlyList<PostEventLabStation> labStations,
            IReadOnlyList<PostEventImmunizationStation> immunizationStations,
            IReadOnlyDictionary<long, string?>? eventServiceTypesByDetailId = null)
        {
            if (eventServices == null || eventServices.Count == 0)
            {
                return;
            }

            foreach (var service in eventServices)
            {
                var serviceType = ResolveServiceType(service, eventServiceTypesByDetailId);
                service.PostEventNumbers = CountCompletedForEventService(
                    service.EventService,
                    serviceType,
                    labStations,
                    immunizationStations);
            }
        }

        private static string? ResolveServiceType(
            PostEventServiceDetailDto service,
            IReadOnlyDictionary<long, string?>? eventServiceTypesByDetailId)
        {
            if (eventServiceTypesByDetailId != null &&
                eventServiceTypesByDetailId.TryGetValue(service.EventServiceDetailId, out var type))
            {
                return type;
            }

            return InferServiceType(service.EventService);
        }

        private static string? InferServiceType(string? eventService)
        {
            if (string.IsNullOrWhiteSpace(eventService))
            {
                return null;
            }

            return IsKnownImmunizationEventService(eventService) ? "Immunizations" : "Labs";
        }

        private static int CountCompletedForEventService(
            string? eventService,
            string? serviceType,
            IReadOnlyList<PostEventLabStation> labStations,
            IReadOnlyList<PostEventImmunizationStation> immunizationStations)
        {
            if (string.IsNullOrWhiteSpace(eventService))
            {
                return 0;
            }

            if (IsLabService(serviceType))
            {
                return labStations.Count(lab => IsLabItemComplete(lab, eventService));
            }

            if (IsImmunizationService(eventService, serviceType))
            {
                return immunizationStations.Count(imm => IsImmunizationItemComplete(imm, eventService));
            }

            return 0;
        }

        private static bool IsLabService(string? serviceType) =>
            string.Equals(serviceType, "Labs", StringComparison.OrdinalIgnoreCase);

        private static bool IsImmunizationService(string? eventService, string? serviceType)
        {
            if (string.Equals(serviceType, "Immunizations", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (IsLabService(serviceType))
            {
                return false;
            }

            return IsKnownImmunizationEventService(eventService);
        }

        private static bool IsKnownImmunizationEventService(string? eventService)
        {
            if (string.IsNullOrWhiteSpace(eventService))
            {
                return false;
            }

            return eventService.Equals("Hep B", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("Hep A", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("MMR", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("Tdap", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("Influenza", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("Flu", StringComparison.OrdinalIgnoreCase) ||
                eventService.Equals("Varicella", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLabItemComplete(PostEventLabStation lab, string eventService)
        {
            var status = GetLabStatusForEventService(lab, eventService);
            return !string.IsNullOrWhiteSpace(status) &&
                LabFinishedStatuses.Contains(status, StringComparer.Ordinal);
        }

        private static string? GetLabStatusForEventService(PostEventLabStation lab, string eventService)
        {
            if (eventService.Equals("HIV Testing", StringComparison.OrdinalIgnoreCase))
            {
                return lab.HivStatus;
            }

            if (eventService.Equals("G6PD testing", StringComparison.OrdinalIgnoreCase))
            {
                return lab.G6pdStatus;
            }

            if (eventService.Equals("Blood Typing (ABO)", StringComparison.OrdinalIgnoreCase))
            {
                return lab.AboStatus;
            }

            if (eventService.Equals("Sickle Cell Trait", StringComparison.OrdinalIgnoreCase))
            {
                return lab.SickleCellStatus;
            }

            if (eventService.Equals("DNA", StringComparison.OrdinalIgnoreCase))
            {
                return lab.DnaStatus;
            }

            if (eventService.Equals("Lipid Panel", StringComparison.OrdinalIgnoreCase))
            {
                return lab.LipidPanelStatus;
            }

            if (eventService.Equals("Pregnancy Test", StringComparison.OrdinalIgnoreCase))
            {
                return lab.PregnancyStatus;
            }

            return null;
        }

        private static bool IsImmunizationItemComplete(PostEventImmunizationStation imm, string eventService)
        {
            var status = GetImmunizationStatusForEventService(imm, eventService);
            return PostEventImmunizationStationStatusHelper.IsVaccineFinishedForOverall(status);
        }

        private static string? GetImmunizationStatusForEventService(
            PostEventImmunizationStation imm,
            string eventService)
        {
            if (eventService.Equals("Hep B", StringComparison.OrdinalIgnoreCase))
            {
                return imm.HepBStatus;
            }

            if (eventService.Equals("Hep A", StringComparison.OrdinalIgnoreCase))
            {
                return imm.HepAStatus;
            }

            if (eventService.Equals("MMR", StringComparison.OrdinalIgnoreCase))
            {
                return imm.MmrStatus;
            }

            if (eventService.Equals("Tdap", StringComparison.OrdinalIgnoreCase))
            {
                return imm.TetTdpStatus;
            }

            if (eventService.Equals("Influenza", StringComparison.OrdinalIgnoreCase))
            {
                return imm.FluStatus;
            }

            if (eventService.Equals("Varicella", StringComparison.OrdinalIgnoreCase))
            {
                return imm.VaricellaStatus;
            }

            return null;
        }
    }
}
