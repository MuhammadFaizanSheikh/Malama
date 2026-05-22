using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class Sf600ImmunizationPdfGenerator : ISf600ImmunizationPdfGenerator
    {
        private const int MaxImmunizations = 6;
        private const int Page1SlotCount = 4;
        private const int Page2SlotCount = 2;

        private readonly ILogger<Sf600ImmunizationPdfGenerator> _logger;

        static Sf600ImmunizationPdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Sf600ImmunizationPdfGenerator(ILogger<Sf600ImmunizationPdfGenerator> logger)
        {
            _logger = logger;
        }

        public Task<byte[]> GenerateAsync(PostEventImmunizationStationAnalysisDto analysisDto) =>
            Task.Run(() => Generate(analysisDto));

        public byte[] Generate(PostEventImmunizationStationAnalysisDto analysisDto)
        {
            const string methodName = nameof(Generate);

            var entries = BuildEntries(analysisDto);
            if (entries.Count == 0)
            {
                throw new InvalidOperationException("No immunization data available for SF600 generation.");
            }

            if (entries.Count > MaxImmunizations)
            {
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, SF600 supports {Max} immunizations; {Count} found. Extra entries omitted.",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName,
                    MaxImmunizations,
                    entries.Count);

                entries = entries.Take(MaxImmunizations).ToList();
            }

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.MarginHorizontal(24);
                        page.MarginVertical(18);
                        page.DefaultTextStyle(Sf600Styles.Body);
                        page.Content().Column(column =>
                        {
                            column.Spacing(4);
                            ComposePageHeader(column, isBackPage: false);
                            ComposePrivacyStatement(column);
                            ComposeChronologicalTitle(column);
                            ComposeTableColumnHeaders(column);
                            ComposePatientIdentification(column, analysisDto.ServiceMember);
                            ComposeFormFooter(column, isBackPage: false);

                            for (var slot = 0; slot < Page1SlotCount; slot++)
                            {
                                var entry = slot < entries.Count ? entries[slot] : null;
                                ComposeImmunizationBlock(column, slot + 1, entry);
                            }
                        });
                    });

                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.MarginHorizontal(24);
                        page.MarginVertical(18);
                        page.DefaultTextStyle(Sf600Styles.Body);
                        page.Content().Column(column =>
                        {
                            column.Spacing(4);
                            ComposePageHeader(column, isBackPage: true);
                            ComposeTableColumnHeaders(column);
                            ComposeFormFooter(column, isBackPage: true);

                            for (var slot = 0; slot < Page2SlotCount; slot++)
                            {
                                var entryIndex = Page1SlotCount + slot;
                                var entry = entryIndex < entries.Count ? entries[entryIndex] : null;
                                ComposeImmunizationBlock(column, entryIndex + 1, entry);
                            }
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, SF600 PDF generated with QuestPDF. ImmunizationCount={EntryCount}",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName,
                    entries.Count);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, SF600 PDF generation failed",
                    nameof(Sf600ImmunizationPdfGenerator),
                    methodName);

                throw new Exception("SF600 PDF generation failed: " + ex.Message, ex);
            }
        }

        private static void ComposePageHeader(ColumnDescriptor column, bool isBackPage)
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("FOR OFFICIAL USE ONLY").Style(Sf600Styles.SmallBold);
                row.RelativeItem().AlignRight().Text("When Filled Out").Style(Sf600Styles.Small);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("PREVIOUS EDITION IS NOT USABLE").Style(Sf600Styles.Small);
                row.RelativeItem().AlignRight().Text("AUTHORIZED FOR LOCAL REPRODUCTION").Style(Sf600Styles.Small);
            });

            column.Item().PaddingTop(4).AlignCenter().Text("MEDICAL RECORD").Style(Sf600Styles.Title);
        }

        private static void ComposePrivacyStatement(ColumnDescriptor column)
        {
            column.Item().PaddingTop(2).Text(
                    "PRIVACY ACT STATEMENT: This information is subject to the Privacy Act of 1974 (5 U.S.C. Section 552a). " +
                    "This information may be provided to appropriate Government agencies when relevant to civil, criminal or regulatory investigations or prosecutions. " +
                    "The Social Security Number, authorized by Public Law 93-579 Section 7 (b) and Executive Order 9397, is used as a unique identifier " +
                    "to distinguish between employees with the same names and birth dates and to ensure that each individual's record in the system is complete " +
                    "and accurate and the information is properly attributed.")
                .Style(Sf600Styles.FinePrint);
        }

        private static void ComposeChronologicalTitle(ColumnDescriptor column)
        {
            column.Item().PaddingTop(6).AlignCenter()
                .Text("CHRONOLOGICAL RECORD OF MEDICAL CARE")
                .Style(Sf600Styles.SectionTitle);
        }

        private static void ComposeTableColumnHeaders(ColumnDescriptor column)
        {
            column.Item().PaddingTop(4).Border(1).BorderColor(Sf600Styles.BorderColor).Row(row =>
            {
                row.ConstantItem(95).BorderRight(1).BorderColor(Sf600Styles.BorderColor)
                    .Padding(4).AlignMiddle().Text("DATE").Style(Sf600Styles.LabelBold);

                row.RelativeItem().Padding(4).AlignMiddle().Text(
                        "SYMPTOMS, DIAGNOSIS, TREATMENT, TREATING ORGANIZATION (Sign each entry)")
                    .Style(Sf600Styles.LabelBold);
            });
        }

        private static void ComposePatientIdentification(ColumnDescriptor column, ServiceMembersChildDto? member)
        {
            column.Item().PaddingTop(4).Border(1).BorderColor(Sf600Styles.BorderColor).Padding(6).Column(box =>
            {
                box.Spacing(3);

                box.Item().Text(
                        "PATIENT'S IDENTIFICATION: (For typed or written entries, give: Name - last, first, middle; ID NUMBER or Social Security Number; Gender; Date of Birth; Rank/Grade.)")
                    .Style(Sf600Styles.Label);

                box.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => LabelValue(c, "Name:", member?.FullName));
                    row.RelativeItem().Element(c => LabelValue(c, "ID NUMBER:", member?.DodId));
                });

                box.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => LabelValue(c, "Date of Birth:", member?.Dob));
                    row.RelativeItem().Element(c => LabelValue(c, "Gender:", member?.Sex));
                });

                box.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => LabelValue(c, "Rank/Grade:", null));
                    row.RelativeItem().Element(c => LabelValue(c, "Barcode:", member?.Barcode));
                });

                box.Item().PaddingTop(2).Row(row =>
                {
                    row.ConstantItem(120).Text("HOSPITAL OR MEDICAL FACILITY").Style(Sf600Styles.Label);
                    row.RelativeItem().Text("SPONSOR'S NAME").Style(Sf600Styles.Label);
                });

                box.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("STATUS").Style(Sf600Styles.Label);
                    row.RelativeItem().Text("DEPARTMENT/SERVICE").Style(Sf600Styles.Label);
                });

                box.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("RELATIONSHIP TO SPONSOR").Style(Sf600Styles.Label);
                    row.RelativeItem().Text("RECORDS MAINTAINED AT").Style(Sf600Styles.Label);
                });

                box.Item().Row(row =>
                {
                    row.RelativeItem().Text("REGISTER NUMBER").Style(Sf600Styles.Label);
                    row.RelativeItem().Text("WARD NUMBER").Style(Sf600Styles.Label);
                });

                box.Item().Text("SOCIAL SECURITY/ID NUMBER").Style(Sf600Styles.Label);
            });

            column.Item().PaddingTop(6).AlignCenter()
                .Text("CHRONOLOGICAL RECORD OF MEDICAL CARE")
                .Style(Sf600Styles.SectionTitle);
        }

        private static void ComposeFormFooter(ColumnDescriptor column, bool isBackPage)
        {
            column.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("Medical Record").Style(Sf600Styles.LabelBold);
                row.RelativeItem().AlignRight().Text(
                        isBackPage
                            ? "STANDARD FORM 600 (REV. 8/2018) BACK"
                            : "STANDARD FORM 600 (REV. 8/2018)")
                    .Style(Sf600Styles.LabelBold);
            });

            if (!isBackPage)
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Prescribed by GSA/ICMR").Style(Sf600Styles.Small);
                    row.RelativeItem().AlignRight().Text("FIRMR (41 CFR) 201-9.202-1").Style(Sf600Styles.Small);
                });
            }
        }

        private static void ComposeImmunizationBlock(
            ColumnDescriptor column,
            int immunizationNumber,
            Sf600ImmunizationEntry? entry)
        {
            column.Item().PaddingTop(4).Border(1).BorderColor(Sf600Styles.BorderColor).Row(row =>
            {
                row.ConstantItem(95).BorderRight(1).BorderColor(Sf600Styles.BorderColor)
                    .Padding(4).AlignTop().Column(dateCol =>
                    {
                        dateCol.Item().Text(FormatDateTime(entry?.GivenDateTime)).Style(Sf600Styles.Value);
                        dateCol.Item().PaddingTop(8).Text("M M  D D  Y Y Y Y").Style(Sf600Styles.FinePrint);
                    });

                row.RelativeItem().Padding(6).Column(treatmentCol =>
                {
                    treatmentCol.Spacing(3);

                    treatmentCol.Item().Text($"Immunization #{immunizationNumber}")
                        .Style(Sf600Styles.ImmunizationTitle);

                    if (!string.IsNullOrWhiteSpace(entry?.VaccineTitle))
                    {
                        treatmentCol.Item().Text($"Vaccine: {entry.VaccineTitle}").Style(Sf600Styles.ValueBold);
                    }

                    treatmentCol.Item().Row(r =>
                    {
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Type:", entry?.AdministrationType));
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Lot#:", entry?.LotNo));
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Expiry Date:", FormatDate(entry?.ExpirationDate)));
                    });

                    treatmentCol.Item().Row(r =>
                    {
                        r.RelativeItem(2).Element(c =>
                            InlineLabelValue(c, "Site: L / R", FormatSite(entry?.Site)));
                        r.RelativeItem(3).Element(c =>
                            InlineLabelValue(c, "Type: IM / SQ / ID", entry?.AdministrationType));
                    });

                    treatmentCol.Item().Text("M M  D D  Y Y Y Y").Style(Sf600Styles.FinePrint);

                    treatmentCol.Item().Element(c =>
                        InlineLabelValue(c, "BodyPart: Shdr/Other (please list):", entry?.BodyPart));

                    treatmentCol.Item().Row(r =>
                    {
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Manufacturer:", entry?.Manufacturer));
                        r.ConstantItem(120).Element(c => InlineLabelValue(c, "Dose #:", FormatDose(entry?.Dose, entry?.Unit)));
                    });

                    treatmentCol.Item().Row(r =>
                    {
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Notes:", null));
                        r.RelativeItem().Element(c => InlineLabelValue(c, "Provided by:", entry?.StaffName));
                    });
                });
            });
        }

        private static void LabelValue(IContainer container, string label, string? value)
        {
            container.Row(row =>
            {
                row.AutoItem().Text(label + " ").Style(Sf600Styles.Label);
                row.RelativeItem().Text(value ?? string.Empty).Style(Sf600Styles.Value);
            });
        }

        private static void InlineLabelValue(IContainer container, string label, string? value)
        {
            container.Text(text =>
            {
                text.Span(label + " ").Style(Sf600Styles.Label);
                text.Span(string.IsNullOrWhiteSpace(value) ? " " : value).Style(Sf600Styles.Value);
            });
        }

        public static List<Sf600ImmunizationEntry> BuildEntries(PostEventImmunizationStationAnalysisDto analysisDto)
        {
            return analysisDto.GetVaccineCards()
                .Select(card => new Sf600ImmunizationEntry
                {
                    VaccineTitle = card.Title,
                    Manufacturer = card.Detail.Manufacturer,
                    Dose = card.Detail.Dose,
                    Unit = card.Detail.Unit,
                    LotNo = card.Detail.LotNo,
                    ExpirationDate = card.Detail.ExpirationDate,
                    AdministrationType = card.Detail.Type,
                    BodyPart = card.Detail.BodyPart,
                    Site = card.Detail.Site,
                    StaffName = card.Detail.StaffName,
                    GivenDateTime = card.Detail.GivenDateTime
                })
                .ToList();
        }

        private static string FormatDate(DateTime? value) =>
            value?.ToString("MM/dd/yyyy") ?? string.Empty;

        private static string FormatDateTime(DateTime? value) =>
            value?.ToString("MM/dd/yyyy hh:mm tt") ?? string.Empty;

        private static string FormatDose(string? dose, string? unit)
        {
            var parts = new[] { dose?.Trim(), unit?.Trim() }
                .Where(p => !string.IsNullOrWhiteSpace(p));

            return string.Join(" ", parts);
        }

        private static string FormatSite(string? site)
        {
            if (string.IsNullOrWhiteSpace(site))
            {
                return string.Empty;
            }

            if (site.Contains("right", StringComparison.OrdinalIgnoreCase))
            {
                return "R";
            }

            if (site.Contains("left", StringComparison.OrdinalIgnoreCase))
            {
                return "L";
            }

            return site;
        }

        private static class Sf600Styles
        {
            public static readonly string BorderColor = Colors.Black;

            public static TextStyle Body => TextStyle.Default.FontSize(8).FontFamily(Fonts.CourierNew);

            public static TextStyle FinePrint => TextStyle.Default.FontSize(6).FontFamily(Fonts.CourierNew);

            public static TextStyle Small => TextStyle.Default.FontSize(7).FontFamily(Fonts.CourierNew);

            public static TextStyle SmallBold => TextStyle.Default.FontSize(7).FontFamily(Fonts.CourierNew).SemiBold();

            public static TextStyle Label => TextStyle.Default.FontSize(7).FontFamily(Fonts.CourierNew);

            public static TextStyle LabelBold => TextStyle.Default.FontSize(7).FontFamily(Fonts.CourierNew).SemiBold();

            public static TextStyle Value => TextStyle.Default.FontSize(8).FontFamily(Fonts.CourierNew);

            public static TextStyle ValueBold => TextStyle.Default.FontSize(8).FontFamily(Fonts.CourierNew).SemiBold();

            public static TextStyle Title => TextStyle.Default.FontSize(11).FontFamily(Fonts.CourierNew).SemiBold();

            public static TextStyle SectionTitle => TextStyle.Default.FontSize(9).FontFamily(Fonts.CourierNew).SemiBold();

            public static TextStyle ImmunizationTitle => TextStyle.Default.FontSize(8).FontFamily(Fonts.CourierNew).SemiBold();
        }
    }
}
