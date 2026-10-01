using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Malama.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalTreatmentService : IDentalTreatmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DentalTreatmentService> _logger;
        private const string CLASSNAME = nameof(DentalTreatmentService);

        public DentalTreatmentService(
            ILogger<DentalTreatmentService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetByServiceMembersChildIdAsync);

            try
            {
                var treatment = await _unitOfWork.DentalTreatment
                    .GetWithIncludeNoTracking(
                        e => e.ServiceMembersChildId == serviceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth,
                        e => e.AnesthesiaRecords,
                        e => e.Prescriptions,
                        e => e.OverallNotes)
                    .FirstOrDefaultAsync();

                if (treatment == null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No dental treatment found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                treatment.Findings = treatment.Findings?.OrderBy(f => f.SortOrder).ToList() ?? new List<DentalTreatmentFinding>();
                treatment.SelectedTeeth = treatment.SelectedTeeth?.OrderBy(t => t.ToothNumber).ToList() ?? new List<DentalTreatmentSelectedTooth>();
                treatment.AnesthesiaRecords = treatment.AnesthesiaRecords?.OrderBy(a => a.SortOrder).ToList() ?? new List<DentalTreatmentAnesthesia>();
                treatment.Prescriptions = treatment.Prescriptions?.OrderBy(p => p.SortOrder).ToList() ?? new List<DentalTreatmentPrescription>();
                treatment.OverallNotes = treatment.OverallNotes?.OrderBy(n => n.SortOrder).ToList() ?? new List<DentalTreatmentOverallNote>();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loaded dental treatment Id={TreatmentId} for ServiceMembersChildId={ServiceMembersChildId}. FindingCount={FindingCount}, ToothCount={ToothCount}",
                    CLASSNAME, methodName, treatment.Id, serviceMembersChildId,
                    treatment.Findings.Count, treatment.SelectedTeeth.Count);

                return treatment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to load dental treatment for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        public async Task<DentalTreatmentCoordinator?> GetCoordinatorByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetCoordinatorByServiceMembersChildIdAsync);

            try
            {
                var coordinator = await _unitOfWork.DentalTreatmentCoordinator
                    .GetWithIncludeNoTracking(
                        e => e.ServiceMembersChildId == serviceMembersChildId,
                        e => e.Appointments)
                    .FirstOrDefaultAsync();

                if (coordinator == null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No treatment coordinator record for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                if (coordinator.Appointments != null && coordinator.Appointments.Count > 0)
                {
                    var appointmentIds = coordinator.Appointments.Select(a => a.Id).ToList();
                    var links = await _unitOfWork.DentalAppointmentFinding
                        .GetWithIncludeNoTracking(f => appointmentIds.Contains(f.AppointmentId))
                        .ToListAsync();

                    foreach (var appt in coordinator.Appointments)
                    {
                        appt.Findings = links.Where(l => l.AppointmentId == appt.Id).ToList();
                    }

                    coordinator.Appointments = coordinator.Appointments
                        .OrderBy(a => a.SortOrder)
                        .ThenBy(a => a.Id)
                        .ToList();
                }

                return coordinator;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to load treatment coordinator for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        public async Task<List<TreatmentCoordinatorEventAppointmentDto>> GetEventAppointmentsExcludingAsync(
            long eventId,
            long excludeServiceMembersChildId)
        {
            const string methodName = nameof(GetEventAppointmentsExcludingAsync);

            try
            {
                if (eventId <= 0)
                {
                    return new List<TreatmentCoordinatorEventAppointmentDto>();
                }

                var appointments = await _unitOfWork.DentalAppointment
                    .GetWithIncludeNoTracking(
                        a => a.DentalTreatmentCoordinator.ServiceMembersChild.ServiceMembersParent.EventManagement.Id == eventId
                             && a.DentalTreatmentCoordinator.ServiceMembersChildId != excludeServiceMembersChildId,
                        a => a.DentalTreatmentCoordinator,
                        a => a.DentalTreatmentCoordinator.ServiceMembersChild)
                    .ToListAsync();

                if (appointments.Count == 0)
                {
                    return new List<TreatmentCoordinatorEventAppointmentDto>();
                }

                var staffIds = appointments.Select(a => a.EventStaffId).Distinct().ToList();
                var staffRows = await _unitOfWork.EventStaff
                    .GetAllWithConditionNoTracking(s => staffIds.Contains(s.Id))
                    .ToListAsync();
                var dentistNameById = staffRows.ToDictionary(
                    s => s.Id,
                    s => DentalExamSignatureHelper.FormatEventStaffDisplayName(s));

                return appointments
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentStartTime)
                    .ThenBy(a => a.Id)
                    .Select(a =>
                    {
                        dentistNameById.TryGetValue(a.EventStaffId, out var dentistName);
                        return TreatmentCoordinatorAppointmentHelper.ToEventDto(
                            a,
                            a.DentalTreatmentCoordinator?.ServiceMembersChild?.FullName ?? string.Empty,
                            dentistName);
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to load event appointments for EventId={EventId}",
                    CLASSNAME, methodName, eventId);
                throw;
            }
        }

        public async Task<long?> TryGetEventStaffIdForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var staff = await _unitOfWork.EventStaff
                .GetWithIncludeNoTracking(es => es.UserId == userId)
                .FirstOrDefaultAsync();

            return staff?.Id > 0 ? staff.Id : null;
        }

        public async Task<bool> IsEligibleForDentalTreatmentAsync(
            long serviceMembersChildId,
            long eventId,
            long eventStaffId)
        {
            if (serviceMembersChildId <= 0 || eventId <= 0 || eventStaffId <= 0)
            {
                return false;
            }

            return await _unitOfWork.ServiceMembersChild
                .GetWithIncludeNoTracking(
                    c => c.Id == serviceMembersChildId &&
                         c.ServiceMembersParent.EventManagement.Id == eventId &&
                         c.CheckIn == AppConstants.YesNo.Yes &&
                         c.DentalTreatmentCoordinatorRecord != null &&
                         c.DentalTreatmentCoordinatorRecord.Appointments.Any(a => a.EventStaffId == eventStaffId))
                .AnyAsync();
        }

        public async Task<HashSet<long>> GetAssignedExamFindingIdsAsync(
            long serviceMembersChildId,
            long eventStaffId)
        {
            var groups = await GetDentistAppointmentFindingGroupsAsync(serviceMembersChildId, eventStaffId);
            return groups
                .SelectMany(g => g.ExamFindingIds ?? new List<long>())
                .Where(id => id > 0)
                .ToHashSet();
        }

        public async Task<List<DentalTreatmentDentistAppointmentGroupDto>> GetDentistAppointmentFindingGroupsAsync(
            long serviceMembersChildId,
            long eventStaffId)
        {
            if (serviceMembersChildId <= 0 || eventStaffId <= 0)
            {
                return new List<DentalTreatmentDentistAppointmentGroupDto>();
            }

            var appointments = await _unitOfWork.DentalAppointment
                .GetWithIncludeNoTracking(
                    a => a.EventStaffId == eventStaffId
                         && a.DentalTreatmentCoordinator.ServiceMembersChildId == serviceMembersChildId)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.SortOrder)
                .ThenBy(a => a.Id)
                .ToListAsync();

            if (appointments.Count == 0)
            {
                return new List<DentalTreatmentDentistAppointmentGroupDto>();
            }

            // Stable time ordering for same-day appointments (stored as display strings).
            appointments = appointments
                .OrderBy(a => a.AppointmentDate.Date)
                .ThenBy(a => TryParseAppointmentStartTime(a.AppointmentStartTime) ?? TimeSpan.MaxValue)
                .ThenBy(a => a.SortOrder)
                .ThenBy(a => a.Id)
                .ToList();

            var appointmentIds = appointments.Select(a => a.Id).ToList();
            var links = await _unitOfWork.DentalAppointmentFinding
                .GetWithIncludeNoTracking(f => appointmentIds.Contains(f.AppointmentId))
                .ToListAsync();

            var groups = appointments.Select(a => new DentalTreatmentDentistAppointmentGroupDto
            {
                AppointmentId = a.Id,
                AppointmentDate = a.AppointmentDate,
                AppointmentStartTime = a.AppointmentStartTime ?? string.Empty,
                AppointmentDuration = a.AppointmentDuration ?? string.Empty,
                SortOrder = a.SortOrder,
                ExamFindingIds = links
                    .Where(l => l.AppointmentId == a.Id && l.DentalFindingId > 0)
                    .Select(l => l.DentalFindingId)
                    .Distinct()
                    .ToList()
            }).ToList();

            _logger.LogInformation(
                "{ClassName}, {MethodName}, ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}, AppointmentCount={AppointmentCount}, FindingCount={FindingCount}",
                CLASSNAME,
                nameof(GetDentistAppointmentFindingGroupsAsync),
                serviceMembersChildId,
                eventStaffId,
                groups.Count,
                groups.Sum(g => g.ExamFindingIds.Count));

            return groups;
        }

        private static TimeSpan? TryParseAppointmentStartTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateTime.TryParse(
                    value.Trim(),
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsed))
            {
                return parsed.TimeOfDay;
            }

            return null;
        }

        public async Task SaveOrUpdateFromFormDataAsync(
            DentalTreatmentStationSaveDto dto,
            string userName,
            string userId,
            IReadOnlySet<long> assignedExamFindingIds,
            long eventStaffId)
        {
            const string methodName = nameof(SaveOrUpdateFromFormDataAsync);
            IDbContextTransaction? transaction = null;
            var assignedIds = assignedExamFindingIds != null
                ? new HashSet<long>(assignedExamFindingIds)
                : new HashSet<long>();

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Begin save. ServiceMembersChildId={ServiceMembersChildId}, User={User}, EventStaffId={EventStaffId}, RequiresDds={RequiresDds}, DdsAcknowledged={DdsAcknowledged}",
                    CLASSNAME,
                    methodName,
                    dto.ServiceMembersChildId,
                    userName,
                    eventStaffId,
                    dto.RequiresDdsAcknowledgement,
                    dto.DdsAcknowledged);

                if (dto.RequiresDdsAcknowledgement && !dto.DdsAcknowledged)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, DDS acknowledgment required but not checked. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}",
                        CLASSNAME, methodName, dto.ServiceMembersChildId, eventStaffId);
                    throw new InvalidOperationException(
                        "DDS acknowledgment is required before saving Dental Treatment for this dentist.");
                }

                dto.Findings = DentalTreatmentJson.ParseList<DentalTreatmentFindingFormDto>(dto.FindingsJson);
                dto.AnesthesiaRecords = DentalTreatmentJson.ParseList<DentalTreatmentAnesthesiaDto>(dto.AnesthesiaJson);
                dto.Prescriptions = DentalTreatmentJson.ParseList<DentalTreatmentPrescriptionDto>(dto.PrescriptionsJson);
                dto.OverallNotes = DentalTreatmentJson.ParseList<DentalTreatmentOverallNoteDto>(dto.OverallNotesJson);
                dto.PsrSelectedTeeth = DentalTreatmentValidator.NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                transaction = await _unitOfWork.BeginTransactionAsync();

                var existing = await _unitOfWork.DentalTreatment
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth,
                        e => e.AnesthesiaRecords,
                        e => e.Prescriptions,
                        e => e.OverallNotes)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    _mapper.Map(dto, existing);
                    existing.UpdatedBy = userId;
                    existing.UpdatedOn = DateTime.Now;

                    ReplaceChildren(existing, dto, userId, assignedIds);
                    existing.Status = DentalTreatmentValidator.ComputeStatusFromPersisted(
                        existing.SmFinalClassification,
                        existing.Findings);

                    await ApplyDdsAcknowledgementToConsentAsync(
                        dto.ServiceMembersChildId,
                        eventStaffId,
                        dto.RequiresDdsAcknowledgement,
                        dto.DdsAcknowledged,
                        userId);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Dental treatment updated. Id={TreatmentId}, ServiceMembersChildId={ServiceMembersChildId}, User={User}, Status={Status}, FindingCount={FindingCount}, ToothCount={ToothCount}, DdsAcknowledged={DdsAcknowledged}",
                        CLASSNAME, methodName, existing.Id, dto.ServiceMembersChildId, userName, existing.Status,
                        existing.Findings?.Count ?? 0, dto.PsrSelectedTeeth.Count, dto.DdsAcknowledged);
                    return;
                }

                var entity = _mapper.Map<DentalTreatment>(dto);
                entity.AddedBy = userId;
                entity.AddedOn = DateTime.Now;

                await _unitOfWork.DentalTreatment.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                ReplaceChildren(entity, dto, userId, assignedIds);
                entity.Status = DentalTreatmentValidator.ComputeStatusFromPersisted(
                    entity.SmFinalClassification,
                    entity.Findings);

                await ApplyDdsAcknowledgementToConsentAsync(
                    dto.ServiceMembersChildId,
                    eventStaffId,
                    dto.RequiresDdsAcknowledgement,
                    dto.DdsAcknowledged,
                    userId);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental treatment created. Id={TreatmentId}, ServiceMembersChildId={ServiceMembersChildId}, User={User}, Status={Status}, FindingCount={FindingCount}, ToothCount={ToothCount}, DdsAcknowledged={DdsAcknowledged}",
                    CLASSNAME, methodName, entity.Id, dto.ServiceMembersChildId, userName, entity.Status,
                    entity.Findings?.Count ?? 0, dto.PsrSelectedTeeth.Count, dto.DdsAcknowledged);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Transaction rolled back for ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx,
                            "{ClassName}, {MethodName}, Failed to rollback dental treatment transaction for ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save dental treatment for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private async Task ApplyDdsAcknowledgementToConsentAsync(
            long serviceMembersChildId,
            long eventStaffId,
            bool requiresDdsAcknowledgement,
            bool ddsAcknowledged,
            string userId)
        {
            const string methodName = nameof(ApplyDdsAcknowledgementToConsentAsync);

            if (!requiresDdsAcknowledgement || eventStaffId <= 0)
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Skipped. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}, RequiresDds={RequiresDds}",
                    CLASSNAME, methodName, serviceMembersChildId, eventStaffId, requiresDdsAcknowledgement);
                return;
            }

            var consent = await _unitOfWork.TreatmentConsent
                .GetWithIncludeTracking(x => x.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();

            if (consent == null)
            {
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, TreatmentConsent row not found. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}",
                    CLASSNAME, methodName, serviceMembersChildId, eventStaffId);
                throw new InvalidOperationException(
                    "Treatment Consent record was not found for DDS acknowledgment.");
            }

            var forms = TreatmentConsentFileSaveCoordinator.ParseTreatmentForms(consent.DentalTreatmentFormsJson);
            var form = forms.FirstOrDefault(f => f.EventStaffId == eventStaffId);
            if (form == null)
            {
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, Dental Treatment Consent form not found for EventStaffId={EventStaffId}, ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, eventStaffId, serviceMembersChildId);
                throw new InvalidOperationException(
                    "Dental Treatment Consent Form was not found for the current dentist.");
            }

            if (ddsAcknowledged)
            {
                if (!TreatmentConsentHelper.IsFormSigned(form.IsSigned, form.SignatureFileName))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, DDS acknowledgment blocked — SM signature missing. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}",
                        CLASSNAME, methodName, serviceMembersChildId, eventStaffId);
                    throw new InvalidOperationException(
                        "The Dental Treatment Consent Form must be signed by the service member before DDS acknowledgment.");
                }

                form.DdsAcknowledged = true;
                form.DdsAcknowledgedByUserId = userId;
                form.DdsAcknowledgedByEventStaffId = eventStaffId;
                form.DdsAcknowledgedOn ??= DateTime.Now;
            }
            else
            {
                form.DdsAcknowledged = false;
                form.DdsAcknowledgedByUserId = null;
                form.DdsAcknowledgedByEventStaffId = null;
                form.DdsAcknowledgedOn = null;
            }

            // Replace this dentist form in the list while preserving others.
            for (var i = 0; i < forms.Count; i++)
            {
                if (forms[i].EventStaffId == eventStaffId)
                {
                    forms[i] = form;
                    break;
                }
            }

            consent.DentalTreatmentFormsJson = System.Text.Json.JsonSerializer.Serialize(
                forms.Select(StripTreatmentFormForConsentJson).ToList());
            consent.UpdatedBy = userId;
            consent.UpdatedOn = DateTime.Now;

            _logger.LogInformation(
                "{ClassName}, {MethodName}, DDS acknowledgment applied. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}, DdsAcknowledged={DdsAcknowledged}, DdsAcknowledgedOn={DdsAcknowledgedOn}",
                CLASSNAME,
                methodName,
                serviceMembersChildId,
                eventStaffId,
                form.DdsAcknowledged,
                form.DdsAcknowledgedOn);
        }

        private static TreatmentConsentDentalTreatmentFormDto StripTreatmentFormForConsentJson(
            TreatmentConsentDentalTreatmentFormDto form)
        {
            return new TreatmentConsentDentalTreatmentFormDto
            {
                EventStaffId = form.EventStaffId,
                DentistName = form.DentistName,
                Item1Mark = form.Item1Mark,
                Fillings = form.Fillings,
                Crowns = form.Crowns,
                Extractions = form.Extractions,
                Impacted = form.Impacted,
                RootCanal = form.RootCanal,
                Fmd = form.Fmd,
                OtherTreatment = form.OtherTreatment,
                OtherTreatmentText = form.OtherTreatmentText,
                Item1Initials = form.Item1Initials,
                Item2Mark = form.Item2Mark,
                Item2Initials = form.Item2Initials,
                Item3Mark = form.Item3Mark,
                RemovalTeeth = form.RemovalTeeth,
                Item3Initials = form.Item3Initials,
                Item4Mark = form.Item4Mark,
                Item4Initials = form.Item4Initials,
                Item5Mark = form.Item5Mark,
                Item5Initials = form.Item5Initials,
                Item6Mark = form.Item6Mark,
                OtherSituation = form.OtherSituation,
                Item6Initials = form.Item6Initials,
                SignatureFileName = form.SignatureFileName,
                IsSigned = form.IsSigned,
                DdsAcknowledged = form.DdsAcknowledged,
                DdsAcknowledgedByUserId = form.DdsAcknowledgedByUserId,
                DdsAcknowledgedByEventStaffId = form.DdsAcknowledgedByEventStaffId,
                DdsAcknowledgedOn = form.DdsAcknowledged ? form.DdsAcknowledgedOn : null
            };
        }

        public async Task ApplyCoordinatorSectionAsync(
            long serviceMembersChildId,
            bool isTreatmentRequired,
            string? comments,
            string status,
            string userName,
            string userId,
            long eventStaffId,
            IReadOnlyList<TreatmentCoordinatorDocumentMetaDto> documents,
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> appointments,
            IReadOnlyDictionary<string, long> findingIdByClientKey,
            bool saveChanges = true)
        {
            const string methodName = nameof(ApplyCoordinatorSectionAsync);

            try
            {
                if (eventStaffId <= 0)
                {
                    throw new InvalidOperationException(
                        "Treatment Coordinator EventStaff Id is required.");
                }

                var now = DentalFindingMapper.NormalizeDateTime(DateTime.Now);
                var trimmedComments = string.IsNullOrWhiteSpace(comments) ? null : comments.Trim();
                var resolvedStatus = string.Equals(status?.Trim(), AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase)
                    ? AppConstants.Status.Completed
                    : AppConstants.Status.Pending;
                var documentsJson = TreatmentCoordinatorDocumentFileSaveCoordinator.SerializeDocuments(documents);

                var existing = await _unitOfWork.DentalTreatmentCoordinator
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == serviceMembersChildId,
                        e => e.Appointments)
                    .FirstOrDefaultAsync();

                DentalTreatmentCoordinator coordinator;
                if (existing != null)
                {
                    existing.IsTreatmentRequired = isTreatmentRequired;
                    existing.TreatmentCoordinatorUserId = userId;
                    existing.TreatmentCoordinatorEventStaffId = eventStaffId;
                    existing.TreatmentCoordinatorDateTime = now;
                    existing.TreatmentCoordinatorComments = trimmedComments;
                    existing.DocumentsJson = documentsJson;
                    existing.Status = resolvedStatus;
                    existing.UpdatedBy = userId;
                    existing.UpdatedOn = now;
                    coordinator = existing;
                }
                else
                {
                    coordinator = new DentalTreatmentCoordinator
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        IsTreatmentRequired = isTreatmentRequired,
                        Status = resolvedStatus,
                        TreatmentCoordinatorUserId = userId,
                        TreatmentCoordinatorEventStaffId = eventStaffId,
                        TreatmentCoordinatorDateTime = now,
                        TreatmentCoordinatorComments = trimmedComments,
                        DocumentsJson = documentsJson,
                        AddedBy = userId,
                        AddedOn = now,
                        Appointments = new List<DentalAppointment>()
                    };
                    await _unitOfWork.DentalTreatmentCoordinator.AddAsync(coordinator);
                }

                ReplaceCoordinatorAppointments(coordinator, appointments, findingIdByClientKey);

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Treatment Coordinator section applied for ServiceMembersChildId={ServiceMembersChildId} by EventStaffId={EventStaffId}. AppointmentCount={AppointmentCount}, DocumentCount={DocumentCount}, SaveChanges={SaveChanges}",
                    CLASSNAME, methodName, serviceMembersChildId, eventStaffId,
                    appointments?.Count ?? 0, documents?.Count ?? 0, saveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to apply Treatment Coordinator section for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        private void ReplaceCoordinatorAppointments(
            DentalTreatmentCoordinator coordinator,
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> appointments,
            IReadOnlyDictionary<string, long> findingIdByClientKey)
        {
            coordinator.Appointments ??= new List<DentalAppointment>();

            var existingAppointments = coordinator.Appointments.ToList();
            if (existingAppointments.Count > 0)
            {
                var existingIds = existingAppointments.Select(a => a.Id).Where(id => id > 0).ToList();
                if (existingIds.Count > 0)
                {
                    var existingLinks = _unitOfWork.DentalAppointmentFinding
                        .GetAllWithConditionNoTracking(f => existingIds.Contains(f.AppointmentId))
                        .ToList();
                    if (existingLinks.Count > 0)
                    {
                        _unitOfWork.DentalAppointmentFinding.RemoveRange(existingLinks);
                    }
                }

                _unitOfWork.DentalAppointment.RemoveRange(existingAppointments);
                coordinator.Appointments.Clear();
            }

            var sortOrder = 0;
            foreach (var appt in appointments ?? Array.Empty<TreatmentCoordinatorAppointmentJsonDto>())
            {
                if (!long.TryParse((appt.AssignedDentist ?? string.Empty).Trim(), out var staffId) || staffId <= 0)
                {
                    continue;
                }

                if (!TreatmentCoordinatorAppointmentHelper.TryParseAppointmentDate(appt.AppointmentDate, out var date))
                {
                    continue;
                }

                var entity = new DentalAppointment
                {
                    EventStaffId = staffId,
                    AppointmentDate = date,
                    AppointmentStartTime = (appt.AppointmentStartTime ?? string.Empty).Trim(),
                    AppointmentDuration = (appt.AppointmentDuration ?? string.Empty).Trim(),
                    SortOrder = sortOrder++,
                    Findings = new List<DentalAppointmentFinding>()
                };

                var keys = (appt.FindingClientKeys ?? new List<string>())
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Select(k => k.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                foreach (var key in keys)
                {
                    if (!findingIdByClientKey.TryGetValue(key, out var findingId) || findingId <= 0)
                    {
                        throw new InvalidOperationException(
                            $"Appointment finding key '{key}' could not be resolved.");
                    }

                    entity.Findings.Add(new DentalAppointmentFinding
                    {
                        DentalFindingId = findingId,
                        FindingClientKey = key
                    });
                }

                coordinator.Appointments.Add(entity);
            }
        }

        private void ReplaceChildren(
            DentalTreatment target,
            DentalTreatmentStationSaveDto dto,
            string userId,
            ISet<long> assignedExamFindingIds)
        {
            ReplaceFindings(target, dto.Findings, userId, assignedExamFindingIds);
            ReplaceSelectedTeeth(target, dto.PsrSelectedTeeth);
            ReplaceAnesthesia(target, dto.AnesthesiaRecords);
            ReplacePrescriptions(target, dto.Prescriptions, userId);
            ReplaceOverallNotes(target, dto.OverallNotes, userId);
        }

        private void ReplaceFindings(
            DentalTreatment target,
            List<DentalTreatmentFindingFormDto> findings,
            string userId,
            ISet<long> assignedExamFindingIds)
        {
            target.Findings ??= new List<DentalTreatmentFinding>();
            var assignedIds = assignedExamFindingIds ?? new HashSet<long>();

            var toRemove = target.Findings
                .Where(f => DentalTreatmentEligibilityHelper.IsInCurrentDentistFindingScope(
                    f,
                    assignedIds,
                    userId))
                .ToList();

            if (toRemove.Count > 0)
            {
                _unitOfWork.DentalTreatmentFinding.RemoveRange(toRemove);
                foreach (var finding in toRemove)
                {
                    target.Findings.Remove(finding);
                }
            }

            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                var origin = ResolveFindingOrigin(finding);
                var isTreatmentOrigin = DentalTreatmentFindingOrigin.IsTreatmentOrigin(origin);
                var examFindingId = !isTreatmentOrigin && finding.DentalFindingId.GetValueOrDefault() > 0
                    ? finding.DentalFindingId
                    : null;

                if (!isTreatmentOrigin)
                {
                    if (examFindingId.GetValueOrDefault() <= 0
                        || !assignedIds.Contains(examFindingId.GetValueOrDefault()))
                    {
                        continue;
                    }
                }
                else if (string.IsNullOrWhiteSpace(finding.FinalDrc))
                {
                    continue;
                }

                var entity = _mapper.Map<DentalTreatmentFinding>(finding);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.DentalFindingId = examFindingId;
                entity.Origin = origin;
                entity.SortOrder = index;
                entity.PostServiceTreatmentJson = DentalTreatmentJson.SerializeList(finding.PostServiceTreatment);
                entity.TreatmentCdtCodesJson = DentalTreatmentJson.SerializeList(finding.TreatmentCdtCodes);
                entity.ProceduredDrc = finding.FinalDrc;
                entity.TreatmentStatus = DentalTreatmentValidator.ResolveFindingTreatmentStatus(finding);
                entity.FindingDateTime = string.IsNullOrWhiteSpace(finding.FindingDateTime) ? null : finding.FindingDateTime.Trim();
                entity.TreatmentDateTime = string.IsNullOrWhiteSpace(finding.TreatmentDateTime) ? null : finding.TreatmentDateTime.Trim();

                var persistDentist = isTreatmentOrigin
                    || string.Equals(finding.TreatmentCompleted, "Yes", StringComparison.OrdinalIgnoreCase);
                entity.DentistProfessional = persistDentist
                    ? (!string.IsNullOrWhiteSpace(finding.DentistProfessional) ? finding.DentistProfessional : userId)
                    : null;

                if (isTreatmentOrigin)
                {
                    entity.IsPrimaryTooth = finding.IsPrimaryTooth;
                    entity.AffectedTooth = finding.AffectedTooth?.Trim();
                    entity.DiseaseConditionType = finding.DiseaseConditionType?.Trim();
                    entity.DentistProfessional = !string.IsNullOrWhiteSpace(finding.DentistProfessional)
                        ? finding.DentistProfessional
                        : userId;
                }
                else
                {
                    // Exam-linked rows keep clinical source of truth on DentalFinding.
                    entity.IsPrimaryTooth = false;
                    entity.AffectedTooth = null;
                    entity.DiseaseConditionType = null;
                }

                target.Findings.Add(entity);
            }
        }

        private static string ResolveFindingOrigin(DentalTreatmentFindingFormDto finding)
        {
            if (!string.IsNullOrWhiteSpace(finding.Origin))
            {
                if (DentalTreatmentFindingOrigin.IsTreatmentOrigin(finding.Origin))
                {
                    return DentalTreatmentFindingOrigin.Treatment;
                }

                if (DentalTreatmentFindingOrigin.IsExamOrigin(finding.Origin))
                {
                    return DentalTreatmentFindingOrigin.Exam;
                }
            }

            if (finding.IsTreatmentOnly || finding.DentalFindingId.GetValueOrDefault() <= 0)
            {
                return DentalTreatmentFindingOrigin.Treatment;
            }

            return DentalTreatmentFindingOrigin.Exam;
        }

        private void ReplaceSelectedTeeth(DentalTreatment target, List<int> toothNumbers)
        {
            target.SelectedTeeth ??= new List<DentalTreatmentSelectedTooth>();
            if (target.SelectedTeeth.Count > 0)
            {
                _unitOfWork.DentalTreatmentSelectedTooth.RemoveRange(target.SelectedTeeth.ToList());
                target.SelectedTeeth.Clear();
            }

            foreach (var toothNumber in toothNumbers)
            {
                target.SelectedTeeth.Add(new DentalTreatmentSelectedTooth
                {
                    DentalTreatmentId = target.Id,
                    ToothNumber = toothNumber
                });
            }
        }

        private void ReplaceAnesthesia(DentalTreatment target, List<DentalTreatmentAnesthesiaDto> records)
        {
            target.AnesthesiaRecords ??= new List<DentalTreatmentAnesthesia>();
            if (target.AnesthesiaRecords.Count > 0)
            {
                _unitOfWork.DentalTreatmentAnesthesia.RemoveRange(target.AnesthesiaRecords.ToList());
                target.AnesthesiaRecords.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentAnesthesia>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.CarpulesByTypeJson = DentalTreatmentJson.SerializeDictionary(record.CarpulesByType);
                target.AnesthesiaRecords.Add(entity);
            }
        }

        private void ReplacePrescriptions(DentalTreatment target, List<DentalTreatmentPrescriptionDto> records, string userId)
        {
            target.Prescriptions ??= new List<DentalTreatmentPrescription>();
            if (target.Prescriptions.Count > 0)
            {
                _unitOfWork.DentalTreatmentPrescription.RemoveRange(target.Prescriptions.ToList());
                target.Prescriptions.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentPrescription>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.PrescribedBy = !string.IsNullOrWhiteSpace(record.PrescribedBy)
                    ? record.PrescribedBy
                    : userId;
                target.Prescriptions.Add(entity);
            }
        }

        private void ReplaceOverallNotes(DentalTreatment target, List<DentalTreatmentOverallNoteDto> records, string userId)
        {
            target.OverallNotes ??= new List<DentalTreatmentOverallNote>();
            if (target.OverallNotes.Count > 0)
            {
                _unitOfWork.DentalTreatmentOverallNote.RemoveRange(target.OverallNotes.ToList());
                target.OverallNotes.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentOverallNote>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.Dentist = !string.IsNullOrWhiteSpace(record.Dentist)
                    ? record.Dentist
                    : userId;
                target.OverallNotes.Add(entity);
            }
        }
    }
}
