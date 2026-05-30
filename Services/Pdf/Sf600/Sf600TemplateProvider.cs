namespace Malama.Services.Pdf.Sf600;

public sealed class Sf600TemplateProvider : ISf600TemplateProvider
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<Sf600TemplateProvider> _logger;

    public Sf600TemplateProvider(IWebHostEnvironment environment, ILogger<Sf600TemplateProvider> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public string GetTemplatePath()
    {
        var path = Path.Combine(_environment.WebRootPath, Sf600PdfConstants.TemplateRelativePath);
        if (!File.Exists(path))
        {
            _logger.LogError("SF600 blank template not found at {TemplatePath}", path);
            throw new FileNotFoundException("SF600 blank template PDF was not found.", path);
        }

        return path;
    }
}
