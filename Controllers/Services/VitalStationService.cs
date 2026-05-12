using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class VitalStationService : IVitalStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<VitalStationService> _logger;
        private const string CLASSNAME = nameof(VitalStationService);

        /// <summary>Normal if systolic is under 140 and diastolic under 90 (same thresholds used for each attempt).</summary>
        private const int NormalSystolicMax = 140;
        private const int NormalDiastolicMax = 90;

        private const int WaitBetweenReadingsMinutes = 15;
        private const int MaxBpAttempts = 3;

        public VitalStationService(
            ILogger<VitalStationService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<VitalStationVM> GetVitalStationByServiceMemberChildIdAsync(long serviceMemberChildId)
        {
            string methodName = nameof(GetVitalStationByServiceMemberChildIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching VitalStation. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMemberChildId);

                var child = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMemberChildId,
                        c => c.ServiceMembersParent.EventManagement,
                        c => c.VitalStationRecord)
                    .Include(c => c.VitalStationRecord!)
                        .ThenInclude(v => v.BloodPressureReadings)
                    .FirstOrDefaultAsync();

                if (child == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Service member child not found. ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMemberChildId);
                    return null;
                }

                var vm = new VitalStationVM
                {
                    EventId = child.ServiceMembersParent.EventManagement.Id,
                    EventID = child.ServiceMembersParent.EventManagement.EventID,
                    ServiceMembersChild = new ServiceMembersChildDto
                    {
                        FullName = child.FullName,
                        DodId = child.DodId,
                        Barcode = child.Barcode,
                        Dob = child.Dob,
                        Age = child.Age,
                        Sex = child.Sex
                    },
                    VitalStationDto = MapToDto(child.VitalStationRecord, child.Id)
                };

                EnrichBpFlowFlags(vm.VitalStationDto);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, VitalStation loaded. ServiceMembersChildId={ServiceMembersChildId}, VitalStationId={VitalStationId}, Status={Status}",
                    CLASSNAME, methodName, serviceMemberChildId, vm.VitalStationDto.Id, vm.VitalStationDto.Status);

                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while fetching VitalStation. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMemberChildId);
                throw;
            }
        }

        private VitalStationDto MapToDto(VitalStation? record, long serviceMembersChildId)
        {
            if (record == null)
            {
                return new VitalStationDto
                {
                    Id = 0,
                    ServiceMembersChildId = serviceMembersChildId,
                    Status = "Pending",
                    TotalReadingsTaken = 0,
                    BloodPressureReadings = new List<VitalStationBpReadingDto>()
                };
            }

            return _mapper.Map<VitalStationDto>(record);
        }

        private static void EnrichBpFlowFlags(VitalStationDto dto)
        {
            dto.NextReadingAfterMinutes = WaitBetweenReadingsMinutes;
            var readings = dto.BloodPressureReadings.OrderBy(r => r.ReadingNumber).ToList();

            if (dto.Status == "Completed")
            {
                dto.IsNextReadingRequired = false;
                dto.NextReadingNumber = null;
                dto.NextBpReadingUnlocked = false;
                dto.NextBpReadingAvailableAt = null;
                dto.Message = dto.FinalBpStatus == "Normal"
                    ? "Vitals completed. Blood pressure is within normal range."
                    : "Vitals completed after three blood pressure readings.";
                return;
            }

            var nextNumber = readings.Count == 0 ? 1 : readings.Max(r => r.ReadingNumber) + 1;
            if (nextNumber > MaxBpAttempts)
            {
                dto.IsNextReadingRequired = false;
                dto.NextReadingNumber = null;
                dto.NextBpReadingUnlocked = false;
                dto.NextBpReadingAvailableAt = null;
                dto.Message = "Maximum blood pressure attempts recorded.";
                return;
            }

            dto.NextReadingNumber = nextNumber;
            dto.IsNextReadingRequired = true;

            if (nextNumber == 1)
            {
                dto.NextBpReadingUnlocked = true;
                dto.NextBpReadingAvailableAt = null;
                dto.Message = "Enter height, weight, and the first blood pressure reading.";
                return;
            }

            var previous = readings.FirstOrDefault(r => r.ReadingNumber == nextNumber - 1);
            if (previous == null)
            {
                dto.NextBpReadingUnlocked = true;
                dto.NextBpReadingAvailableAt = null;
                dto.Message = $"Enter blood pressure reading #{nextNumber}.";
                return;
            }

            var unlockAt = previous.ReadingTakenAt.AddMinutes(WaitBetweenReadingsMinutes);

            dto.NextBpReadingUnlocked = DateTime.Now >= unlockAt;
            // Do not leave a past instant here: the countdown script treats any set value as active and would reload forever once unlocked.
            dto.NextBpReadingAvailableAt = dto.NextBpReadingUnlocked ? null : unlockAt;

            dto.Message = dto.NextBpReadingUnlocked
                ? $"Enter blood pressure reading #{nextNumber}."
                : $"Reading #{nextNumber} is available {WaitBetweenReadingsMinutes} minutes after the previous reading.";
        }

        public async Task AddAsync(VitalStationDto model, string userName)
        {
            string methodName = nameof(AddAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Adding VitalStation. ServiceMembersChildId={ServiceMembersChildId}, User={User}",
                    CLASSNAME, methodName, model.ServiceMembersChildId, userName);

                ValidateHeightWeight(model);

                if (model.PendingSystolic == null || model.PendingDiastolic == null)
                {
                    throw new InvalidOperationException("Systolic and diastolic values are required for the first blood pressure reading.");
                }

                ValidateBpValues(model.PendingSystolic.Value, model.PendingDiastolic.Value);

                var now = DateTime.Now;
                var sys = model.PendingSystolic.Value;
                var dia = model.PendingDiastolic.Value;
                var normal = IsBpNormal(sys, dia);

                var entity = _mapper.Map<VitalStation>(model);

                var readingDto = new VitalStationBpReadingDto
                {
                    ReadingNumber = 1,
                    Systolic = sys,
                    Diastolic = dia,
                    ReadingStatus = normal ? "Normal" : "High",
                    IsRetakeRequired = !normal,
                    ReadingTakenAt = now,
                    Remarks = normal ? null : "High - 2nd reading is required"
                };

                var reading = _mapper.Map<VitalStationBloodPressureReading>(readingDto);
                entity.BloodPressureReadings = new List<VitalStationBloodPressureReading> { reading };
                entity.TotalReadingsTaken = 1;
                entity.FinalBpStatus = normal ? "Normal" : null;
                entity.Status = normal ? "Completed" : "Pending";
                entity.AddedOn = now;
                entity.AddedBy = userName;

                await _unitOfWork.VitalStation.AddAsync(entity);
                await _unitOfWork.SaveAsync();
                model.Id = entity.Id;

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, VitalStation added successfully. Id={Id}, ServiceMembersChildId={ServiceMembersChildId}, Status={Status}, User={User}",
                    CLASSNAME, methodName, entity.Id, entity.ServiceMembersChildId, entity.Status, userName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex,
                    "{ClassName}, {MethodName}, Validation or business rule failed while adding VitalStation. ServiceMembersChildId={ServiceMembersChildId}, User={User}",
                    CLASSNAME, methodName, model?.ServiceMembersChildId, userName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while adding VitalStation. ServiceMembersChildId={ServiceMembersChildId}, User={User}",
                    CLASSNAME, methodName, model?.ServiceMembersChildId, userName);
                throw;
            }
        }

        public async Task UpdateAsync(VitalStationDto model, string userName)
        {
            string methodName = nameof(UpdateAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Updating VitalStation. Id={Id}, ServiceMembersChildId={ServiceMembersChildId}, User={User}",
                    CLASSNAME, methodName, model.Id, model.ServiceMembersChildId, userName);

                ValidateHeightWeight(model);

                var entity = await _unitOfWork.VitalStation
                    .GetWithIncludeTracking(v => v.Id == model.Id, v => v.BloodPressureReadings)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    throw new InvalidOperationException("Vital station record was not found.");
                }

                if (entity.ServiceMembersChildId != model.ServiceMembersChildId)
                {
                    throw new InvalidOperationException("Service member does not match this vital station record.");
                }

                if (entity.Status == "Completed")
                {
                    throw new InvalidOperationException("This vital station record is already completed.");
                }

                if (model.PendingSystolic == null || model.PendingDiastolic == null)
                {
                    throw new InvalidOperationException("Systolic and diastolic values are required.");
                }

                ValidateBpValues(model.PendingSystolic.Value, model.PendingDiastolic.Value);

                var readings = entity.BloodPressureReadings.OrderBy(r => r.ReadingNumber).ToList();
                var nextNumber = readings.Count == 0 ? 1 : readings.Max(r => r.ReadingNumber) + 1;

                if (nextNumber > MaxBpAttempts)
                {
                    throw new InvalidOperationException("No further blood pressure readings are allowed.");
                }

                if (nextNumber > 1)
                {
                    var previous = readings.First(r => r.ReadingNumber == nextNumber - 1);
                    var unlockAt = previous.ReadingTakenAt.AddMinutes(WaitBetweenReadingsMinutes);
                    if (DateTime.Now < unlockAt)
                    {
                        throw new InvalidOperationException(
                            $"The next blood pressure reading can be taken after {WaitBetweenReadingsMinutes} minutes from the previous reading.");
                    }
                }

                var now = DateTime.Now;
                var sys = model.PendingSystolic.Value;
                var dia = model.PendingDiastolic.Value;
                var normal = IsBpNormal(sys, dia);

                string? remarks = null;
                if (!normal)
                {
                    remarks = nextNumber switch
                    {
                        1 => "High - 2nd reading is required",
                        2 => "High - 3rd reading is required",
                        _ => "High"
                    };
                }

                var readingDto = new VitalStationBpReadingDto
                {
                    ReadingNumber = nextNumber,
                    Systolic = sys,
                    Diastolic = dia,
                    ReadingStatus = normal ? "Normal" : "High",
                    IsRetakeRequired = !normal && nextNumber < MaxBpAttempts,
                    ReadingTakenAt = now,
                    Remarks = remarks
                };

                var newReading = _mapper.Map<VitalStationBloodPressureReading>(readingDto);
                entity.BloodPressureReadings.Add(newReading);

                _mapper.Map(model, entity);

                entity.TotalReadingsTaken = readings.Count + 1;
                entity.UpdatedOn = now;
                entity.UpdatedBy = userName;

                var anyNormal = entity.BloodPressureReadings.Any(r => IsBpNormal(r.Systolic, r.Diastolic));
                if (anyNormal)
                {
                    entity.FinalBpStatus = "Normal";
                    entity.Status = "Completed";
                }
                else if (entity.TotalReadingsTaken >= MaxBpAttempts)
                {
                    entity.FinalBpStatus = "High";
                    entity.Status = "Completed";
                }
                else
                {
                    entity.FinalBpStatus = null;
                    entity.Status = "Pending";
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, VitalStation updated successfully. Id={Id}, ServiceMembersChildId={ServiceMembersChildId}, Status={Status}, TotalReadingsTaken={TotalReadingsTaken}, User={User}",
                    CLASSNAME, methodName, entity.Id, entity.ServiceMembersChildId, entity.Status, entity.TotalReadingsTaken, userName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex,
                    "{ClassName}, {MethodName}, Validation or business rule failed while updating VitalStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model?.Id, userName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while updating VitalStation. Id={Id}, User={User}",
                    CLASSNAME, methodName, model?.Id, userName);
                throw;
            }
        }

        private static void ValidateHeightWeight(VitalStationDto model)
        {
            if (model.Height == null || model.Weight == null)
            {
                throw new InvalidOperationException("Height and weight are required.");
            }

            if (model.Height <= 0 || model.Weight <= 0)
            {
                throw new InvalidOperationException("Height and weight must be greater than zero.");
            }
        }

        private static void ValidateBpValues(int systolic, int diastolic)
        {
            if (systolic < 40 || systolic > 300 || diastolic < 20 || diastolic > 200)
            {
                throw new InvalidOperationException("Blood pressure values are outside the allowed range.");
            }

            if (diastolic >= systolic)
            {
                throw new InvalidOperationException("Diastolic must be less than systolic.");
            }
        }

        private static bool IsBpNormal(int systolic, int diastolic) =>
            systolic < NormalSystolicMax && diastolic < NormalDiastolicMax;
    }
}
