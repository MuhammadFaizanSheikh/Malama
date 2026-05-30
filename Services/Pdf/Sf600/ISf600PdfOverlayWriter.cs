namespace Malama.Services.Pdf.Sf600;

public interface ISf600PdfOverlayWriter
{
    byte[] WriteOverlays(string templatePath, IReadOnlyList<Sf600OverlayText> overlays);
}
