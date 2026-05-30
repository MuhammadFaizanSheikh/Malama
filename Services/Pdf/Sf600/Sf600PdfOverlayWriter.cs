using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace Malama.Services.Pdf.Sf600;

public sealed class Sf600PdfOverlayWriter : ISf600PdfOverlayWriter
{
    private readonly ISf600PdfFontProvider _fontProvider;
    private readonly ILogger<Sf600PdfOverlayWriter> _logger;

    public Sf600PdfOverlayWriter(ISf600PdfFontProvider fontProvider, ILogger<Sf600PdfOverlayWriter> logger)
    {
        _fontProvider = fontProvider;
        _logger = logger;
    }

    public byte[] WriteOverlays(string templatePath, IReadOnlyList<Sf600OverlayText> overlays)
    {
        var nonEmptyOverlays = overlays.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();
        var pageCount = nonEmptyOverlays.Count == 0
            ? 1
            : nonEmptyOverlays.Max(o => o.PageNumber);

        using var outputStream = new MemoryStream();
        using var reader = new PdfReader(templatePath);
        using var sourceDoc = new PdfDocument(reader);

        var writerProperties = new WriterProperties().SetFullCompressionMode(true);
        using var writer = new PdfWriter(outputStream, writerProperties);
        writer.SetCloseStream(false);

        using (var pdfDoc = new PdfDocument(writer))
        {
            for (var i = 0; i < pageCount; i++)
            {
                sourceDoc.CopyPagesTo(1, 1, pdfDoc);
            }

            var regularFont = _fontProvider.GetDataFont(pdfDoc);
            var boldFont = _fontProvider.GetDataFontBold(pdfDoc);

            foreach (var overlay in nonEmptyOverlays)
            {
                var font = overlay.IsBold ? boldFont : regularFont;
                DrawText(pdfDoc, font, overlay);
            }
        }

        var pdfBytes = outputStream.ToArray();

        _logger.LogDebug(
            "SF600 overlay complete. PageCount={PageCount}, OverlayCount={OverlayCount}, OutputBytes={OutputBytes}",
            pageCount,
            nonEmptyOverlays.Count,
            pdfBytes.Length);

        return pdfBytes;
    }

    private static void DrawText(PdfDocument pdfDoc, PdfFont font, Sf600OverlayText overlay)
    {
        var page = pdfDoc.GetPage(overlay.PageNumber);
        var canvas = new PdfCanvas(page, true);

        canvas.BeginText()
            .SetFontAndSize(font, overlay.FontSize)
            .MoveText(overlay.X, overlay.Y)
            .ShowText(overlay.Text)
            .EndText();
    }
}
