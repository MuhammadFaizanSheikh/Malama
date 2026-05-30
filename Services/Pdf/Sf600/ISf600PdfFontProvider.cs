using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace Malama.Services.Pdf.Sf600;

public interface ISf600PdfFontProvider
{
    PdfFont GetDataFont(PdfDocument pdfDoc);

    PdfFont GetDataFontBold(PdfDocument pdfDoc);
}
