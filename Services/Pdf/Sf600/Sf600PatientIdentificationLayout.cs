namespace Malama.Services.Pdf.Sf600;

/// <summary>
/// Patient identification block at bottom-left of SF600-Template.pdf page 1.
/// </summary>
internal static class Sf600PatientIdentificationLayout
{
    public const float FontSize = 10f;
    public const float LabelX = 25f;
    public const float ValueX = 85f;
    public const float LineHeight = 10f;

    /// <summary>Baseline Y for first data line (Name), below instruction text.</summary>
    public const float FirstLineBaselineY = 106f;

    public static float GetLineBaseline(int lineIndex) =>
        FirstLineBaselineY - (lineIndex * LineHeight);
}
