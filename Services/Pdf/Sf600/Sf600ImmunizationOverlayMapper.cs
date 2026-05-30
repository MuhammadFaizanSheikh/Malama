using Malama.Models;

namespace Malama.Services.Pdf.Sf600;

internal static class Sf600ImmunizationOverlayMapper
{
    public static IReadOnlyList<Sf600OverlayText> Map(
        PostEventImmunizationStationAnalysisDto analysisDto,
        IReadOnlyList<Sf600ImmunizationEntry> entries)
    {
        var totalBlocks = Math.Min(entries.Count, Sf600PdfConstants.MaxImmunizationSlots);
        var overlays = new List<Sf600OverlayText>();

        for (var i = 0; i < totalBlocks; i++)
        {
            overlays.AddRange(MapBlock(entries[i], i));
        }

        overlays.AddRange(MapNotesLabels(totalBlocks));

        var pageCount = Sf600ImmunizationBlockLayout.GetRequiredPageCount(totalBlocks);
        overlays.AddRange(Sf600PatientIdentificationMapper.Map(analysisDto.ServiceMember, pageCount));

        return overlays;
    }

    public static IReadOnlyList<Sf600OverlayText> MapEntries(IReadOnlyList<Sf600ImmunizationEntry> entries)
    {
        var totalBlocks = Math.Min(entries.Count, Sf600PdfConstants.MaxImmunizationSlots);
        var overlays = new List<Sf600OverlayText>();

        for (var i = 0; i < totalBlocks; i++)
        {
            overlays.AddRange(MapBlock(entries[i], i));
        }

        overlays.AddRange(MapNotesLabels(totalBlocks));

        return overlays;
    }

    private static IEnumerable<Sf600OverlayText> MapBlock(Sf600ImmunizationEntry entry, int immunizationIndex)
    {
        var page = Sf600ImmunizationBlockLayout.GetPageNumber(immunizationIndex);
        var localIndex = Sf600ImmunizationBlockLayout.GetLocalImmunizationIndex(immunizationIndex);
        var firstRow = Sf600ImmunizationBlockLayout.GetFirstRowIndexForImmunization(localIndex);

        var givenDate = FormatGivenDate(entry.GivenDateTime);
        if (!string.IsNullOrWhiteSpace(givenDate))
        {
            yield return Text(
                page,
                Sf600ImmunizationBlockLayout.DateX,
                Sf600ImmunizationBlockLayout.GetRowBaseline(page, firstRow),
                givenDate);
        }

        var valuesByLine = new[]
        {
            new[] { FormatDose(entry.Dose, entry.Unit), entry.StaffName ?? string.Empty },
            new[] { entry.VaccineTitle, entry.Manufacturer ?? string.Empty },
            new[] { entry.LotNo ?? string.Empty, FormatDate(entry.ExpirationDate) },
            new[] { entry.Site ?? string.Empty, entry.DisplayBodyPart ?? string.Empty, entry.AdministrationType ?? string.Empty }
        };

        for (var lineIndex = 0; lineIndex < Sf600ImmunizationBlockLayout.ImmunizationLines.Length; lineIndex++)
        {
            var rowIndex = firstRow + lineIndex;
            var baselineY = Sf600ImmunizationBlockLayout.GetRowBaseline(page, rowIndex);
            var fields = Sf600ImmunizationBlockLayout.ImmunizationLines[lineIndex];

            for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
            {
                var field = fields[fieldIndex];
                var label = lineIndex == 0 && fieldIndex == 0
                    ? string.Format(field.Label, entry.SequenceNumber)
                    : field.Label;

                yield return Text(page, field.LabelX, baselineY, label, isBold: true);

                if (lineIndex == 0 && fieldIndex == 0)
                {
                    continue;
                }

                var valueIndex = lineIndex == 0 ? fieldIndex - 1 : fieldIndex;
                var value = valuesByLine[lineIndex][valueIndex];
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                yield return Text(page, field.ValueX, baselineY, value);
            }
        }
    }

    private static IEnumerable<Sf600OverlayText> MapNotesLabels(int immunizationCount)
    {
        var pageCount = Sf600ImmunizationBlockLayout.GetRequiredPageCount(immunizationCount);

        for (var page = 1; page <= pageCount; page++)
        {
            var notesLabel = page == 1 ? "Notes:" : "Notes Con't:";
            yield return Text(
                page,
                Sf600ImmunizationBlockLayout.NotesLabelX,
                Sf600ImmunizationBlockLayout.GetRowBaseline(page, Sf600ImmunizationBlockLayout.NotesRowIndex),
                notesLabel,
                isBold: true);
        }
    }

    private static Sf600OverlayText Text(int page, float x, float y, string text, bool isBold = false) =>
        new()
        {
            PageNumber = page,
            X = x,
            Y = y,
            Text = text.Trim(),
            IsBold = isBold
        };

    private static string FormatGivenDate(DateTime? value) =>
        value?.ToString("MM/dd/yyyy") ?? string.Empty;

    private static string FormatDate(DateTime? value) =>
        value?.ToString("MM/dd/yyyy") ?? string.Empty;

    private static string FormatDose(string? dose, string? unit)
    {
        var parts = new[] { dose?.Trim(), unit?.Trim() }
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return string.Join(" ", parts);
    }
}
