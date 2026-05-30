namespace Malama.Services.Pdf.Sf600;

public sealed class Sf600OverlayText
{
    public required int PageNumber { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required string Text { get; init; }
    public float FontSize { get; init; } = Sf600PdfConstants.DataFontSize;
    public bool IsBold { get; init; }
}
