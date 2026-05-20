using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Malama.Models
{
    public class ImmunizationVaccineDetailDto
    {
        public string? Manufacturer { get; set; }
        public string? Dose { get; set; }
        public string? Unit { get; set; }
        public string? LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Type { get; set; }
        public string? BodyPart { get; set; }
        public string? Site { get; set; }
        public string? StaffName { get; set; }
        public DateTime? GivenDateTime { get; set; }
    }

    public class ImmunizationVaccineCardViewModel
    {
        public string Title { get; set; }
        public string CardId { get; set; }
        public ImmunizationVaccineDetailDto Detail { get; set; } = new();
        public bool DataEntered { get; set; }
        public DateTime? DataEnteredDateTime { get; set; }
        public string DataEnteredFieldName { get; set; }
        public string DataEnteredDateTimeFieldName { get; set; }
        public string VaccineStatusName { get; set; }
        public string? VaccineStatus { get; set; }
    }

    public class PreEventImmunizationStationDto
    {
        public string? HepBNeeded { get; set; }
        public string? HepANeeded { get; set; }
        public string? FluNeeded { get; set; }
        public string? MmrNeeded { get; set; }
        public string? TetTdpNeeded { get; set; }
        public string? VaricellaNeeded { get; set; }

        public ImmunizationVaccineDetailDto? HepB { get; set; }
        public ImmunizationVaccineDetailDto? HepA { get; set; }
        public ImmunizationVaccineDetailDto? Flu { get; set; }
        public ImmunizationVaccineDetailDto? Mmr { get; set; }
        public ImmunizationVaccineDetailDto? TetTdp { get; set; }
        public ImmunizationVaccineDetailDto? Varicella { get; set; }
    }

    public class PostEventImmunizationStationAnalysisDto
    {
        public long EventId { get; set; }
        public string? EventID { get; set; }

        [ValidateNever]
        public ServiceMembersChildDto ServiceMember { get; set; }

        [ValidateNever]
        public PreEventImmunizationStationDto ImmunizationStation { get; set; }

        [ValidateNever]
        public PostEventImmunizationStationDto PostEventImmunizationStation { get; set; } = new();

        public bool ShowHepBSection => ImmunizationStation?.HepBNeeded == AppConstants.Status.Completed;
        public bool ShowHepASection => ImmunizationStation?.HepANeeded == AppConstants.Status.Completed;
        public bool ShowFluSection => ImmunizationStation?.FluNeeded == AppConstants.Status.Completed;
        public bool ShowMmrSection => ImmunizationStation?.MmrNeeded == AppConstants.Status.Completed;
        public bool ShowTetTdpSection => ImmunizationStation?.TetTdpNeeded == AppConstants.Status.Completed;
        public bool ShowVaricellaSection => ImmunizationStation?.VaricellaNeeded == AppConstants.Status.Completed;

        public IEnumerable<ImmunizationVaccineCardViewModel> GetVaccineCards()
        {
            var post = PostEventImmunizationStation;
            var pre = ImmunizationStation;

            if (ShowHepBSection)
            {
                yield return BuildCard("Hep B", pre?.HepB, post.HepBDataEntered, post.HepBDataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.HepBDataEntered),
                    nameof(PostEventImmunizationStationDto.HepBDataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.HepBStatus),
                    post.HepBStatus);
            }
            if (ShowHepASection)
            {
                yield return BuildCard("Hep A", pre?.HepA, post.HepADataEntered, post.HepADataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.HepADataEntered),
                    nameof(PostEventImmunizationStationDto.HepADataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.HepAStatus),
                    post.HepAStatus);
            }
            if (ShowFluSection)
            {
                yield return BuildCard("Flu", pre?.Flu, post.FluDataEntered, post.FluDataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.FluDataEntered),
                    nameof(PostEventImmunizationStationDto.FluDataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.FluStatus),
                    post.FluStatus);
            }
            if (ShowMmrSection)
            {
                yield return BuildCard("MMR", pre?.Mmr, post.MmrDataEntered, post.MmrDataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.MmrDataEntered),
                    nameof(PostEventImmunizationStationDto.MmrDataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.MmrStatus),
                    post.MmrStatus);
            }
            if (ShowTetTdpSection)
            {
                yield return BuildCard("Tdap", pre?.TetTdp, post.TetTdpDataEntered, post.TetTdpDataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.TetTdpDataEntered),
                    nameof(PostEventImmunizationStationDto.TetTdpDataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.TetTdpStatus),
                    post.TetTdpStatus);
            }
            if (ShowVaricellaSection)
            {
                yield return BuildCard("Varicella", pre?.Varicella, post.VaricellaDataEntered, post.VaricellaDataEnteredDateTime,
                    nameof(PostEventImmunizationStationDto.VaricellaDataEntered),
                    nameof(PostEventImmunizationStationDto.VaricellaDataEnteredDateTime),
                    nameof(PostEventImmunizationStationDto.VaricellaStatus),
                    post.VaricellaStatus);
            }
        }

        private static ImmunizationVaccineCardViewModel BuildCard(
            string title,
            ImmunizationVaccineDetailDto? detail,
            bool dataEntered,
            DateTime? dataEnteredDateTime,
            string dataEnteredFieldName,
            string dataEnteredDateTimeFieldName,
            string vaccineStatusName,
            string? vaccineStatus)
        {
            var cardId = dataEnteredFieldName.Replace("DataEntered", "", StringComparison.Ordinal);

            return new ImmunizationVaccineCardViewModel
            {
                Title = title,
                CardId = cardId,
                Detail = detail ?? new ImmunizationVaccineDetailDto(),
                DataEntered = dataEntered,
                DataEnteredDateTime = dataEnteredDateTime,
                DataEnteredFieldName = dataEnteredFieldName,
                DataEnteredDateTimeFieldName = dataEnteredDateTimeFieldName,
                VaccineStatusName = vaccineStatusName,
                VaccineStatus = vaccineStatus
            };
        }
    }

    public class PostEventImmunizationStationDto
    {
        public string SubmissionToken { get; set; }
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }
        public long PostEventManagementId { get; set; }
        public string Status { get; set; } = AppConstants.Status.Pending;

        public bool HepBDataEntered { get; set; }
        public DateTime? HepBDataEnteredDateTime { get; set; }
        public bool HepADataEntered { get; set; }
        public DateTime? HepADataEnteredDateTime { get; set; }
        public bool FluDataEntered { get; set; }
        public DateTime? FluDataEnteredDateTime { get; set; }
        public bool MmrDataEntered { get; set; }
        public DateTime? MmrDataEnteredDateTime { get; set; }
        public bool TetTdpDataEntered { get; set; }
        public DateTime? TetTdpDataEnteredDateTime { get; set; }
        public bool VaricellaDataEntered { get; set; }
        public DateTime? VaricellaDataEnteredDateTime { get; set; }

        public string? HepBStatus { get; set; }
        public string? HepAStatus { get; set; }
        public string? FluStatus { get; set; }
        public string? MmrStatus { get; set; }
        public string? TetTdpStatus { get; set; }
        public string? VaricellaStatus { get; set; }
    }
}
