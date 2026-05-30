using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;

namespace Malama.Services.Pdf.Sf600;

public sealed class Sf600PdfFontProvider : ISf600PdfFontProvider
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<Sf600PdfFontProvider> _logger;
    private readonly Lazy<FontProgram> _regularFontProgram;
    private readonly Lazy<FontProgram> _boldFontProgram;

    public Sf600PdfFontProvider(IWebHostEnvironment environment, ILogger<Sf600PdfFontProvider> logger)
    {
        _environment = environment;
        _logger = logger;
        _regularFontProgram = new Lazy<FontProgram>(() => LoadFontProgram(Sf600PdfConstants.ArialFontRelativePath, "arial.ttf"));
        _boldFontProgram = new Lazy<FontProgram>(() => LoadFontProgram(Sf600PdfConstants.ArialBoldFontRelativePath, "arialbd.ttf"));
    }

    public PdfFont GetDataFont(PdfDocument pdfDoc) => CreateFont(_regularFontProgram.Value);

    public PdfFont GetDataFontBold(PdfDocument pdfDoc) => CreateFont(_boldFontProgram.Value);

    private static PdfFont CreateFont(FontProgram fontProgram) =>
        PdfFontFactory.CreateFont(
            fontProgram,
            PdfEncodings.WINANSI,
            PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

    private FontProgram LoadFontProgram(string relativePath, string windowsFileName)
    {
        var fontPath = ResolveFontPath(relativePath, windowsFileName);
        _logger.LogDebug("Loading SF600 font program from {FontPath}", fontPath);
        return FontProgramFactory.CreateFont(fontPath);
    }

    private string ResolveFontPath(string relativePath, string windowsFileName)
    {
        var embeddedPath = Path.Combine(_environment.WebRootPath, relativePath);
        if (File.Exists(embeddedPath))
        {
            return embeddedPath;
        }

        var windowsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            windowsFileName);

        if (File.Exists(windowsPath))
        {
            _logger.LogWarning(
                "SF600 font not found in wwwroot at {EmbeddedPath}; using system font at {FontPath}",
                embeddedPath,
                windowsPath);
            return windowsPath;
        }

        throw new FileNotFoundException(
            $"Required SF600 font was not found. Expected under wwwroot/{relativePath}.",
            embeddedPath);
    }
}
