using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.XWPF.UserModel;
using System.Xml.Linq;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly IWebHostEnvironment _env;

        public PdfGeneratorService(IWebHostEnvironment env)
        {
            _env = env;
        }

        //public async Task<byte[]> GenerateEventSummaryPdfAsync(FileDataDto dto)
        //{
        //    try
        //    {
        //        using var ms = new MemoryStream();
        //        var document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
        //        PdfWriter.GetInstance(document, ms);
        //        document.Open();

        //        // 🔹 Font variable (change here to update entire PDF)
        //        var mainFont = FontFactory.GetFont(FontFactory.TIMES, 12);

        //        // 🔹 Add Logo
        //        var logoPath = Path.Combine(_env.WebRootPath, "images", "Dawson-Logo.png");
        //        float logoBottomY = document.PageSize.Height - 36; // Default top margin
        //        if (File.Exists(logoPath))
        //        {
        //            var logo = Image.GetInstance(logoPath);
        //            logo.ScaleToFit(60f, 60f); // Adjust size if needed
        //            float logoX = 36f; // Left margin
        //            float logoY = document.PageSize.Height - 36 - logo.ScaledHeight; // Top margin minus logo height
        //            logo.SetAbsolutePosition(logoX, logoY);
        //            document.Add(logo);

        //            // Store bottom Y of logo for spacing
        //            logoBottomY = logoY - 10; // 10 units padding below logo
        //        }

        //        // 🔹 Main Title
        //        var titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 20);
        //        var title = new Paragraph("Event Summary", titleFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER,
        //            SpacingBefore = document.PageSize.Height - logoBottomY + 5 // start just below logo
        //        };
        //        document.Add(title);

        //        // 🔹 Sub Heading
        //        var subFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 14);
        //        var sub = new Paragraph("Service Member Details", subFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER,
        //            SpacingAfter = 20
        //        };
        //        document.Add(sub);

        //        // 🔹 Personal Info Section
        //        var table = new PdfPTable(2)
        //        {
        //            WidthPercentage = 80,
        //            HorizontalAlignment = Element.ALIGN_CENTER
        //        };
        //        table.SetWidths(new float[] { 1f, 2f });

        //        void AddRow(string label, string value)
        //        {
        //            table.AddCell(new PdfPCell(new Phrase(label, mainFont)) { Border = Rectangle.NO_BORDER });
        //            table.AddCell(new PdfPCell(new Phrase(value ?? "", mainFont)) { Border = Rectangle.NO_BORDER });
        //        }

        //        AddRow("Name", dto.FullName);
        //        AddRow("DoD ID / Last 4", $"{dto.DodId}/{dto.Last4}");
        //        AddRow("Event ID", dto.EventId.ToString());
        //        //AddRow("Date of Service", dto.EventDate?.ToString("MM/dd/yyyy"));

        //        document.Add(table);

        //        // 🔹 Later we will add child table sections here

        //        document.Close();
        //        return ms.ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("PDF generation failed: " + ex.Message);
        //    }
        //}

        public async Task<byte[]> GenerateEventSummaryPdfAsync(FileDataDto dto)
        {
            try
            {
                using var ms = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fonts
                var mainFont = FontFactory.GetFont(FontFactory.TIMES, 12);
                var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);

                // 🔹 Add Logo
                var logoPath = Path.Combine(_env.WebRootPath, "images", "Dawson-Logo.png");
                float logoBottomY = document.PageSize.Height - 36;
                if (File.Exists(logoPath))
                {
                    var logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(60f, 60f);
                    float logoX = 36f;
                    float logoY = document.PageSize.Height - 36 - logo.ScaledHeight;
                    logo.SetAbsolutePosition(logoX, logoY);
                    document.Add(logo);

                    logoBottomY = logoY - 10; // padding below logo
                }

                // 🔹 Main Heading
                var titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 20);
                var titleText = new Phrase("Event Summary", titleFont);
                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_CENTER,
                    titleText,
                    document.PageSize.Width / 2,
                    logoBottomY + 40,
                    0
                );

                // 🔹 Sub Heading
                var subFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 14);
                var subText = new Phrase("Service Member Details", subFont);
                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_CENTER,
                    subText,
                    document.PageSize.Width / 2,
                    logoBottomY + 20,
                    0
                );

                // 🔹 Line Separator
                var line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -10);

                // 🔹 Add spacing below headings
                document.Add(new Paragraph("\n") { SpacingAfter = 2f });

                // 🔹 Personal Info Table
                var table = new PdfPTable(2) { WidthPercentage = 80, HorizontalAlignment = Element.ALIGN_LEFT };
                table.SetWidths(new float[] { 1f, 2f });

                void AddRow(string label, string value)
                {
                    table.AddCell(new PdfPCell(new Phrase(label, mainFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        PaddingTop = 2f,
                        PaddingBottom = 2f
                    });
                    table.AddCell(new PdfPCell(new Phrase(value ?? "", mainFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        PaddingTop = 2f,
                        PaddingBottom = 2f
                    });
                }

                document.Add(new Chunk(line));

                AddRow("Name", dto.FullName);
                AddRow("DoD ID / Last 4", $"{dto.DodId}/{dto.Last4}");

                string startDate = "";
                string endDate = "";

                if (!string.IsNullOrEmpty(dto.EventDate) && DateTime.TryParse(dto.EventDate, out var parsedStart))
                    startDate = parsedStart.ToString("MM/dd/yyyy");

                if (!string.IsNullOrEmpty(dto.EventEndDate) && DateTime.TryParse(dto.EventEndDate, out var parsedEnd))
                    endDate = parsedEnd.ToString("MM/dd/yyyy");

                string eventInfo = $"{dto.EventId} ({startDate} - {endDate})";
                AddRow("Event ID", eventInfo);

                document.Add(table);
                //document.Add(new Chunk(line));
                //document.Add(new Paragraph("\n") { SpacingAfter = -10f });

                // 🔹 Immunization Section
                if (dto.Imm == "NEEDED")
                {
                    var immFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 14);
                    document.Add(new Paragraph("Immunization", immFont) { SpacingAfter = 2f });
                    line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, 12);
                    document.Add(new Chunk(line));
                    document.Add(new Paragraph("\n") { SpacingAfter = -10f });

                    void AddImmRow(string vaccineName, string status, string reason = null, string comment = null, DateTime? givenDateTime = null)
                    {
                        var table = new PdfPTable(2) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 60f, 40f });

                        // Status cell
                        table.AddCell(new PdfPCell(new Phrase($"{vaccineName} Needed Status : {status}", mainFont))
                        {
                            Border = Rectangle.NO_BORDER,
                            PaddingTop = 2f,
                            PaddingBottom = 2f
                        });

                        string rightText = "";
                        if (status == "Not Completed" && !string.IsNullOrEmpty(reason))
                            rightText = $"Reason : {reason}";
                        else if (status == "Completed" && givenDateTime.HasValue)
                            rightText = $"Given Date/Time : {givenDateTime.Value.ToString("MM/dd/yyyy hh:mm tt")}";

                        table.AddCell(new PdfPCell(new Phrase(rightText, mainFont))
                        {
                            Border = Rectangle.NO_BORDER,
                            PaddingTop = 2f,
                            PaddingBottom = 2f
                        });

                        document.Add(table);

                        // Comment row if Excused
                        if (reason == "Excused" && !string.IsNullOrEmpty(comment))
                        {
                            var commentTable = new PdfPTable(2) { WidthPercentage = 100 };
                            commentTable.SetWidths(new float[] { 60f, 40f });
                            commentTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });
                            commentTable.AddCell(new PdfPCell(new Phrase($"Comment : {comment}", mainFont))
                            {
                                Border = Rectangle.NO_BORDER,
                                PaddingTop = 2f,
                                PaddingBottom = 2f
                            });
                            document.Add(commentTable);
                        }
                    }

                    // Overall Immunization Status
                    string overallStatus = dto.ImmunizationRecord != null ? dto.ImmunizationRecord.Status : "Pending";
                    AddImmRow("Immunization", overallStatus);

                    
                    // Individual vaccines only if record exists
                    if (dto.ImmunizationRecord != null)
                    {
                        if (dto.HepB == "NEEDED")
                            AddImmRow("HepB", dto.ImmunizationRecord.HepBNeeded,
                                dto.ImmunizationRecord.HepBNeeded == "Not Completed" ? dto.ImmunizationRecord.HepBReason : null,
                                dto.ImmunizationRecord.HepBReason == "Excused" ? dto.ImmunizationRecord.HepBReasonExcusedComments : null,
                                dto.ImmunizationRecord.HepBGivenDateTime);

                        if (dto.Flu == "NEEDED")
                            AddImmRow("Flu", dto.ImmunizationRecord.FluNeeded,
                                dto.ImmunizationRecord.FluNeeded == "Not Completed" ? dto.ImmunizationRecord.FluReason : null,
                                dto.ImmunizationRecord.FluReason == "Excused" ? dto.ImmunizationRecord.FluReasonExcusedComments : null,
                                dto.ImmunizationRecord.FluGivenDateTime);

                        if (dto.Mmr == "NEEDED")
                            AddImmRow("MMR", dto.ImmunizationRecord.MMRNeeded,
                                dto.ImmunizationRecord.MMRNeeded == "Not Completed" ? dto.ImmunizationRecord.MMRReason : null,
                                dto.ImmunizationRecord.MMRReason == "Excused" ? dto.ImmunizationRecord.MMRReasonExcusedComments : null,
                                dto.ImmunizationRecord.MMRGivenDateTime);

                        if (dto.HepA == "NEEDED")
                            AddImmRow("HepA", dto.ImmunizationRecord.HepANeeded,
                                dto.ImmunizationRecord.HepANeeded == "Not Completed" ? dto.ImmunizationRecord.HepAReason : null,
                                dto.ImmunizationRecord.HepAReason == "Excused" ? dto.ImmunizationRecord.HepAReasonExcusedComments : null,
                                dto.ImmunizationRecord.HepAGivenDateTime);

                        if (dto.TetTdp == "NEEDED")
                            AddImmRow("Tet/Tdp", dto.ImmunizationRecord.TetTdpNeeded,
                                dto.ImmunizationRecord.TetTdpNeeded == "Not Completed" ? dto.ImmunizationRecord.TetTdpReason : null,
                                dto.ImmunizationRecord.TetTdpReason == "Excused" ? dto.ImmunizationRecord.TetTdpReasonExcusedComments : null,
                                dto.ImmunizationRecord.TetTdpGivenDateTime);

                        if (dto.Varicella == "NEEDED")
                            AddImmRow("Varicella", dto.ImmunizationRecord.VaricellaNeeded,
                                dto.ImmunizationRecord.VaricellaNeeded == "Not Completed" ? dto.ImmunizationRecord.VaricellaReason : null,
                                dto.ImmunizationRecord.VaricellaReason == "Excused" ? dto.ImmunizationRecord.VaricellaReasonExcusedComments : null,
                                dto.ImmunizationRecord.VaricellaGivenDateTime);
                    }
                }

                document.Close();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("PDF generation failed: " + ex.Message);
            }
        }

        public async Task<byte[]> GenerateHivSignInSheetPdfAsync(List<FileDataDto> dtos, EventManagement eventInfo)
        {
            try
            {
                using var ms = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fonts
                var mainFont = FontFactory.GetFont(FontFactory.TIMES, 11);
                var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 11);
                var titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 20);
                var subFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);

                // 🔹 Heading coordinates
                float yStart = document.PageSize.Height - 36; // top margin
                float currentY = yStart;

                // 🔹 Main Heading
                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_CENTER,
                    new Phrase("HIV Sign-In Sheet", titleFont),
                    document.PageSize.Width / 2,
                    currentY,
                    0
                );

                // Move Y down for subheading
                currentY -= 30; // space below heading

                // 🔹 Sub Heading: Event Location & Date
                string locationText = $"Event Location : {eventInfo.EventCity}, {eventInfo.EventState}";
                string eventIdText = $"Event ID : {eventInfo.EventID}";
                string dateText = $"Date : {eventInfo.EventEndDate:MM/dd/yyyy}";

                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_LEFT,
                    new Phrase($"{locationText}              {eventIdText}", subFont),
                    36,        // left margin
                    currentY,
                    0
                );

                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_RIGHT,
                    new Phrase(dateText, subFont),
                    document.PageSize.Width - 36, // right margin
                    currentY,
                    0
                );

                // Move Y down for table
                currentY -= 40; // increased space to avoid overlap with table

                // 🔹 Add spacing using an empty paragraph to shift the table down
                document.Add(new Paragraph("\n") { SpacingBefore = currentY - 50 }); // adjust extra spacing if needed

                // 🔹 Line separator
                var line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, 0);
                document.Add(new Chunk(line));
                document.Add(new Paragraph("\n") { SpacingAfter = 5f });

                // 🔹 Table with 5 columns
                var table = new PdfPTable(5)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 6f, 28f, 18f, 18f, 30f });

                void AddHeader(string text)
                {
                    table.AddCell(new PdfPCell(new Phrase(text, boldFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f
                    });
                }

                AddHeader("S.No");
                AddHeader("Name (Last, First)");
                AddHeader("FMP / SSN");
                AddHeader("Date of Birth");
                AddHeader("Carebill");

                int index = 1;
                foreach (var dto in dtos)
                {
                    table.AddCell(new PdfPCell(new Phrase(index.ToString(), mainFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f
                    });
                    table.AddCell(new PdfPCell(new Phrase(dto.FullName ?? "", mainFont))
                    {
                        Padding = 5f
                    });
                    table.AddCell(new PdfPCell(new Phrase(dto.FullSsn ?? "", mainFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f
                    });

                    string dob = "";
                    if (!string.IsNullOrEmpty(dto.Dob) && DateTime.TryParse(dto.Dob, out var parsedDob))
                        dob = parsedDob.ToString("MM/dd/yyyy");

                    table.AddCell(new PdfPCell(new Phrase(dob, mainFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f
                    });

                    table.AddCell(new PdfPCell(new Phrase(dto.LabStationRecord?.HivBarcodeCarebill ?? "", mainFont))
                    {
                        Padding = 5f
                    });

                    index++;
                }

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("HIV Sign-In Sheet PDF generation failed: " + ex.Message);
            }
        }










    }

}
