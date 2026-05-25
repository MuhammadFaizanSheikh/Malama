using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class Sf600ImmunizationPdfGenerator : ISf600ImmunizationPdfGenerator
    {
        private const float PageMarginH = 36f;
        private const float PageMarginV = 36f;
        private const float InnerBorderThickness = 0.5f;
        private const float SeparatorBorderThickness = 1.5f;
        private const float FacilityRowMinHeight = 30f;
        private const float FacilityWritingAreaHeight = 14f;

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
                throw new InvalidOperationException("No completed immunization data available for SF600 generation.");
            }

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.MarginHorizontal(PageMarginH);
                        page.MarginVertical(PageMarginV);
                        page.DefaultTextStyle(Sf600Styles.Arial10);
                        page.Content().Element(c => ComposeDocument(c, entries));
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

        private static void ComposeDocument(IContainer container, IReadOnlyList<Sf600ImmunizationEntry> entries)
        {
            container.Column(column =>
            {
                column.Spacing(8);

                ComposeTitleRow(column);
                ComposePrivacyStatement(column);
                ComposeMainTable(column, entries);
            });
        }

        private static void ComposeTitleRow(ColumnDescriptor column)
        {
            column.Item().Border(1).BorderColor(Sf600Styles.LineColor).Row(row =>
            {
                row.RelativeItem(1).BorderRight(1).BorderColor(Sf600Styles.LineColor)
                    .Padding(4).AlignLeft()
                    .Text("MEDICAL RECORD")
                    .Style(Sf600Styles.Arial10Bold);

                row.RelativeItem(1).Padding(4).AlignRight()
                    .Text("CHRONOLOGICAL RECORD OF MEDICAL CARE")
                    .Style(Sf600Styles.Arial10Bold);
            });
        }

        private static void ComposePrivacyStatement(ColumnDescriptor column)
        {
            column.Item().Text(text =>
            {
                text.Span("PRIVACY ACT STATEMENT: ").Style(Sf600Styles.Arial10Bold);
                text.Span(
                        "This information is subject to the Privacy Act of 1974 (5 U.S.C. Section 552a).  This information " +
                        "may be provided to appropriate Government agencies when relevant to civil, criminal or regulatory investigations or prosecutions. " +
                        "The Social Security Number, authorized by Public Law 93-579 Section 7 (b) and Executive Order 9397, is used as a unique " +
                        "identifier to distinguish between employees with the same names and birth dates and to ensure that each individual's record in " +
                        "the system is complete and accurate and the information is properly attributed.")
                    .Style(Sf600Styles.Arial10);
            });
        }

        private static void ComposeMainTable(ColumnDescriptor column, IReadOnlyList<Sf600ImmunizationEntry> entries)
        {
            column.Item().Border(1).BorderColor(Sf600Styles.LineColor).Column(table =>
            {
                table.Item().BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor).Row(row =>
                {
                    row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                        .Padding(4)
                        .Text("DATE")
                        .Style(Sf600Styles.Arial8Bold);

                    row.RelativeItem(3).Padding(4)
                        .Text("SYMPTOMS, DIAGNOSIS, TREATMENT, TREATING ORGANIZATION (Sign each entry)")
                        .Style(Sf600Styles.Arial8Bold);
                });

                foreach (var entry in entries)
                {
                    table.Item().Element(c => ComposeImmunizationBlock(c, entry));
                }

                ComposeFacilityInfoRows(table);
            });
        }

        private static void ComposeFacilityInfoRows(ColumnDescriptor table)
        {
            table.Item().BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor).Row(row =>
            {
                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Element(c => ComposeFacilityCell(c, "HOSPITAL OR MEDICAL FACILITY"));

                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Element(c => ComposeFacilityCell(c, "STATUS"));

                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Element(c => ComposeFacilityCell(c, "DEPARTMENT/SERVICE"));

                row.RelativeItem(1).Element(c => ComposeFacilityCell(c, "RECORDS MAINTAINED AT"));
            });

            table.Item().BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor).Row(row =>
            {
                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Element(c => ComposeFacilityCell(c, "SPONSOR'S NAME"));

                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Element(c => ComposeFacilityCell(c, "SOCIAL SECURITY/ID NUMBER"));

                row.RelativeItem(2).Element(c => ComposeFacilityCell(c, "RELATIONSHIP TO SPONSOR"));
            });

            table.Item().Height(SeparatorBorderThickness).Background(Sf600Styles.LineColor);
        }

        private static void ComposeFacilityCell(IContainer container, string label)
        {
            container.MinHeight(FacilityRowMinHeight).Padding(4).Column(col =>
            {
                col.Item().Text(label).Style(Sf600Styles.Arial7Bold);
                col.Item().PaddingTop(2).MinHeight(FacilityWritingAreaHeight);
            });
        }

        private static void ComposeImmunizationBlock(IContainer container, Sf600ImmunizationEntry entry)
        {
            container.Column(block =>
            {
                ComposeImmunizationDataRow(block, FormatGivenDate(entry.GivenDateTime), drawLineBelow: true,
                    ("Immunization #" + entry.SequenceNumber, null),
                    ("Dose #", FormatDose(entry.Dose, entry.Unit)),
                    ("Provided By", entry.StaffName));

                ComposeImmunizationDataRow(block, dateText: null, drawLineBelow: true,
                    ("Type", entry.VaccineTitle),
                    ("Manufacturer", entry.Manufacturer),
                    null);

                ComposeImmunizationDataRow(block, dateText: null, drawLineBelow: true,
                    ("Lot #", entry.LotNo),
                    ("Expiry Date", FormatDate(entry.ExpirationDate)),
                    null);

                ComposeImmunizationDataRow(block, dateText: null, drawLineBelow: false,
                    ("Site", entry.Site),
                    ("Body Part", entry.DisplayBodyPart),
                    ("Type", entry.AdministrationType));

                ComposeBoldSeparator(block);
                ComposeNotesRows(block);
                ComposeBoldSeparator(block);
            });
        }

        private static void ComposeBoldSeparator(ColumnDescriptor column) =>
            column.Item().Height(SeparatorBorderThickness).Background(Sf600Styles.LineColor);

        private static void ComposeNotesRows(ColumnDescriptor column)
        {
            ComposeNotesRow(column, includeLabel: true, drawLineBelow: true);
            ComposeNotesRow(column, includeLabel: false, drawLineBelow: false);
        }

        private static void ComposeNotesRow(ColumnDescriptor column, bool includeLabel, bool drawLineBelow)
        {
            var rowItem = column.Item();

            if (drawLineBelow)
            {
                rowItem = rowItem.BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor);
            }

            rowItem.Row(row =>
            {
                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Padding(6)
                    .MinHeight(18);

                row.RelativeItem(3).Padding(6).Column(notesCol =>
                {
                    if (includeLabel)
                    {
                        notesCol.Item().Text(text =>
                        {
                            text.Span("Notes : ").Style(Sf600Styles.Arial12Bold);
                        });
                    }

                    notesCol.Item().MinHeight(16).BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor);
                });
            });
        }

        private static void ComposeImmunizationDataRow(
            ColumnDescriptor column,
            string? dateText,
            bool drawLineBelow,
            (string Key, string? Value)? column1,
            (string Key, string? Value)? column2 = null,
            (string Key, string? Value)? column3 = null)
        {
            var rowItem = column.Item();

            if (drawLineBelow)
            {
                rowItem = rowItem.BorderBottom(InnerBorderThickness).BorderColor(Sf600Styles.LineColor);
            }

            rowItem.Row(row =>
            {
                row.RelativeItem(1).BorderRight(InnerBorderThickness).BorderColor(Sf600Styles.LineColor)
                    .Padding(6).AlignMiddle()
                    .Text(dateText ?? string.Empty)
                    .Style(Sf600Styles.Arial12);

                row.RelativeItem(3).Element(c => ComposeAlignedDataLine(c, column1, column2, column3));
            });
        }

        private static void ComposeAlignedDataLine(
            IContainer container,
            (string Key, string? Value)? column1,
            (string Key, string? Value)? column2,
            (string Key, string? Value)? column3)
        {
            container.PaddingVertical(4).PaddingHorizontal(6).Row(row =>
            {
                row.RelativeItem(1).PaddingHorizontal(4).Element(c => ComposeFieldCell(c, column1));
                row.RelativeItem(1).PaddingHorizontal(4).Element(c => ComposeFieldCell(c, column2));
                row.RelativeItem(1).PaddingHorizontal(4).Element(c => ComposeFieldCell(c, column3));
            });
        }

        private static void ComposeFieldCell(IContainer container, (string Key, string? Value)? part)
        {
            if (part == null)
            {
                container.MinHeight(12);
                return;
            }

            if (part.Value.Value == null)
            {
                container.Text(part.Value.Key).Style(Sf600Styles.Arial12Bold);
                return;
            }

            container.Text(text =>
            {
                text.Span(part.Value.Key + " : ").Style(Sf600Styles.Arial12Bold);
                text.Span(part.Value.Value).Style(Sf600Styles.Arial12);
            });
        }

        public static List<Sf600ImmunizationEntry> BuildEntries(PostEventImmunizationStationAnalysisDto analysisDto)
        {
            var sequence = 1;
            var entries = new List<Sf600ImmunizationEntry>();

            foreach (var card in analysisDto.GetVaccineCards())
            {
                if (!IsVaccineEligibleForSf600(card, analysisDto.ImmunizationStation))
                {
                    continue;
                }

                entries.Add(new Sf600ImmunizationEntry
                {
                    SequenceNumber = sequence++,
                    VaccineTitle = card.Title,
                    Manufacturer = card.Detail.Manufacturer,
                    Dose = card.Detail.Dose,
                    Unit = card.Detail.Unit,
                    LotNo = card.Detail.LotNo,
                    ExpirationDate = card.Detail.ExpirationDate,
                    AdministrationType = card.Detail.Type,
                    BodyPart = card.Detail.BodyPart,
                    DisplayBodyPart = FormatBodyPart(card.Detail.BodyPart),
                    Site = card.Detail.Site,
                    StaffName = card.Detail.StaffName,
                    GivenDateTime = card.Detail.GivenDateTime
                });
            }

            return entries;
        }

        private static bool IsVaccineEligibleForSf600(
            ImmunizationVaccineCardViewModel card,
            PreEventImmunizationStationDto? pre)
        {
            if (pre == null)
            {
                return false;
            }

            var neededStatus = card.CardId switch
            {
                "HepB" => pre.HepBNeeded,
                "HepA" => pre.HepANeeded,
                "Flu" => pre.FluNeeded,
                "Mmr" => pre.MmrNeeded,
                "TetTdp" => pre.TetTdpNeeded,
                "Varicella" => pre.VaricellaNeeded,
                _ => null
            };

            return neededStatus == AppConstants.Status.Completed;
        }

        private static string FormatBodyPart(string? bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart))
            {
                return string.Empty;
            }

            if (bodyPart.Equals("Shdr", StringComparison.OrdinalIgnoreCase) ||
                bodyPart.Equals("Shoulder", StringComparison.OrdinalIgnoreCase))
            {
                return "Shdr";
            }

            return bodyPart;
        }

        private static string FormatGivenDate(DateTime? value) =>
            value?.ToString("MM/dd/yyyy") ?? string.Empty;

        private static string FormatDate(DateTime? value) =>
            value?.ToString("MM/dd/yyyy") ?? string.Empty;

        private static string FormatDose(string? dose, string? unit)
        {
            var parts = new[] { dose?.Trim(), unit?.Trim() }
                .Where(p => !string.IsNullOrWhiteSpace(p));

            return string.Join(" ", parts);
        }

        private static class Sf600Styles
        {
            public static readonly string LineColor = Colors.Black;

            public static TextStyle Arial10 =>
                TextStyle.Default.FontSize(10).FontFamily(Fonts.Arial);

            public static TextStyle Arial10Bold =>
                TextStyle.Default.FontSize(10).FontFamily(Fonts.Arial).SemiBold();

            public static TextStyle Arial8Bold =>
                TextStyle.Default.FontSize(8).FontFamily(Fonts.Arial).SemiBold();

            public static TextStyle Arial7Bold =>
                TextStyle.Default.FontSize(7).FontFamily(Fonts.Arial).SemiBold();

            public static TextStyle Arial12 =>
                TextStyle.Default.FontSize(12).FontFamily(Fonts.Arial);

            public static TextStyle Arial12Bold =>
                TextStyle.Default.FontSize(12).FontFamily(Fonts.Arial).SemiBold();
        }
    }
}
