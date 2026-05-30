using ExcelFilesCompiler.Utilities;
using Malama.Models;

namespace Malama.Services.Pdf.Sf600;

internal static class Sf600ImmunizationEntryBuilder
{
    public static List<Sf600ImmunizationEntry> BuildEntries(PostEventImmunizationStationAnalysisDto analysisDto)
    {
        var sequence = 1;
        var entries = new List<Sf600ImmunizationEntry>();

        foreach (var card in analysisDto.GetVaccineCards())
        {
            if (!IsVaccineEligibleForSf600(card, analysisDto.ImmunizationStation))
            {
                continue;
            }

            entries.Add(new Sf600ImmunizationEntry
            {
                SequenceNumber = sequence++,
                VaccineTitle = card.Title,
                Manufacturer = card.Detail.Manufacturer,
                Dose = card.Detail.Dose,
                Unit = card.Detail.Unit,
                LotNo = card.Detail.LotNo,
                ExpirationDate = card.Detail.ExpirationDate,
                AdministrationType = card.Detail.Type,
                BodyPart = card.Detail.BodyPart,
                DisplayBodyPart = FormatBodyPart(card.Detail.BodyPart),
                Site = card.Detail.Site,
                StaffName = card.Detail.StaffName,
                GivenDateTime = card.Detail.GivenDateTime
            });
        }

        return entries;
    }

    private static bool IsVaccineEligibleForSf600(
        ImmunizationVaccineCardViewModel card,
        PreEventImmunizationStationDto? pre)
    {
        if (pre == null)
        {
            return false;
        }

        var neededStatus = card.CardId switch
        {
            "HepB" => pre.HepBNeeded,
            "HepA" => pre.HepANeeded,
            "Flu" => pre.FluNeeded,
            "Mmr" => pre.MmrNeeded,
            "TetTdp" => pre.TetTdpNeeded,
            "Varicella" => pre.VaricellaNeeded,
            _ => null
        };

        return neededStatus == AppConstants.Status.Completed;
    }

    private static string FormatBodyPart(string? bodyPart)
    {
        if (string.IsNullOrWhiteSpace(bodyPart))
        {
            return string.Empty;
        }

        if (bodyPart.Equals("Shdr", StringComparison.OrdinalIgnoreCase) ||
            bodyPart.Equals("Shoulder", StringComparison.OrdinalIgnoreCase))
        {
            return "Shdr";
        }

        return bodyPart;
    }
}
