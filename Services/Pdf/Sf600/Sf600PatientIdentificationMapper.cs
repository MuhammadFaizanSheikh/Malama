using Malama.Models;

namespace Malama.Services.Pdf.Sf600;

internal static class Sf600PatientIdentificationMapper
{
    public static IEnumerable<Sf600OverlayText> Map(ServiceMembersChildDto? serviceMember, int pageCount)
    {
        if (serviceMember == null || pageCount <= 0)
        {
            yield break;
        }

        var lines = new (string Label, string? Value)[]
        {
            ("Name :", serviceMember.FullName),
            ("DoD ID :", serviceMember.DodId),
            ("SSN :", serviceMember.FullSsn),
            ("Sex :", serviceMember.Sex),
            ("DOB :", serviceMember.Dob),
            ("Rank :", serviceMember.Rank)
        };

        for (var page = 1; page <= pageCount; page++)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                var baselineY = Sf600PatientIdentificationLayout.GetLineBaseline(i);

                yield return new Sf600OverlayText
                {
                    PageNumber = page,
                    X = Sf600PatientIdentificationLayout.LabelX,
                    Y = baselineY,
                    Text = lines[i].Label,
                    FontSize = Sf600PatientIdentificationLayout.FontSize,
                    IsBold = true
                };

                var value = lines[i].Value?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                yield return new Sf600OverlayText
                {
                    PageNumber = page,
                    X = Sf600PatientIdentificationLayout.ValueX,
                    Y = baselineY,
                    Text = value,
                    FontSize = Sf600PatientIdentificationLayout.FontSize
                };
            }
        }
    }
}
