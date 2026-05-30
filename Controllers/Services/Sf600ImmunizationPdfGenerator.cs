using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Malama.Services.Pdf.Sf600;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class Sf600ImmunizationPdfGenerator : ISf600ImmunizationPdfGenerator
    {
        private readonly ISf600TemplateProvider _templateProvider;
        private readonly ISf600PdfOverlayWriter _overlayWriter;
        private readonly ILogger<Sf600ImmunizationPdfGenerator> _logger;

        public Sf600ImmunizationPdfGenerator(
            ISf600TemplateProvider templateProvider,
            ISf600PdfOverlayWriter overlayWriter,
            ILogger<Sf600ImmunizationPdfGenerator> logger)
        {
            _templateProvider = templateProvider;
            _overlayWriter = overlayWriter;
            _logger = logger;
        }

        public Task<byte[]> GenerateAsync(PostEventImmunizationStationAnalysisDto analysisDto) =>
            Task.Run(() => Generate(analysisDto));

        public byte[] Generate(PostEventImmunizationStationAnalysisDto analysisDto)
        {
            const string methodName = nameof(Generate);

            var entries = Sf600ImmunizationEntryBuilder.BuildEntries(analysisDto);
            if (entries.Count == 0)
            {
                throw new InvalidOperationException("No completed immunization data available for SF600 generation.");
            }

            var entriesToPrint = entries;
            if (entries.Count > Sf600PdfConstants.MaxImmunizationSlots)
            {
                _logger.LogWarning(
                    "{ClassName}.{MethodName} - {EntryCount} immunizations exceed template capacity of {MaxSlots}; only the first {MaxSlots} will be printed.",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName,
                    entries.Count,
                    Sf600PdfConstants.MaxImmunizationSlots,
                    Sf600PdfConstants.MaxImmunizationSlots);
                entriesToPrint = entries.Take(Sf600PdfConstants.MaxImmunizationSlots).ToList();
            }

            try
            {
                var templatePath = _templateProvider.GetTemplatePath();
                var overlays = Sf600ImmunizationOverlayMapper.Map(analysisDto, entriesToPrint);
                var pdfBytes = _overlayWriter.WriteOverlays(templatePath, overlays);

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - SF600 PDF generated with blank template overlay. ImmunizationCount={EntryCount}",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName,
                    entriesToPrint.Count);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - SF600 PDF generation failed",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName);

                throw new Exception("SF600 PDF generation failed: " + ex.Message, ex);
            }
        }
    }
}
