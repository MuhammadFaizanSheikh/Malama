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
                var line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2);
                //document.Add(new Chunk(line));
                //document.Add(Chunk.NEWLINE);

                // 🔹 Add spacing below headings to avoid overlap with table
                document.Add(new Paragraph("\n\n\n")); // adjust spacing as needed

                // 🔹 Personal Info Table
                var table = new PdfPTable(2) { WidthPercentage = 80, HorizontalAlignment = Element.ALIGN_LEFT };
                table.SetWidths(new float[] { 1f, 2f });

                void AddRow(string label, string value)
                {
                    table.AddCell(new PdfPCell(new Phrase(label, mainFont)) { Border = Rectangle.NO_BORDER });
                    table.AddCell(new PdfPCell(new Phrase(value ?? "", mainFont)) { Border = Rectangle.NO_BORDER });
                }

                document.Add(new Chunk(line));
                document.Add(Chunk.NEWLINE);

                AddRow("Name", dto.FullName);
                AddRow("DoD ID / Last 4", $"{dto.DodId}/{dto.Last4}");
                AddRow("Event ID", dto.EventId.ToString());

                document.Add(table);

                // 🔹 Line Separator
                //document.Add(new Chunk(line));
                //document.Add(Chunk.NEWLINE);

                // 🔹 Conditional Immunization Section
                if (dto.ImmunizationRecord != null)
                {
                    var immFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);
                    document.Add(new Paragraph("Immunization", immFont));
                    document.Add(new Chunk(line));
                    document.Add(Chunk.NEWLINE);

                    void AddImmRow(string vaccineName, string status, string reason = null, string comment = null)
                    {
                        // Table with 2 columns
                        var table = new PdfPTable(2) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 60f, 40f }); // adjust widths as needed

                        // Status in left cell
                        table.AddCell(new PdfPCell(new Phrase($"{vaccineName} Needed Status : {status}", mainFont))
                        {
                            Border = Rectangle.NO_BORDER
                        });

                        // Reason in right cell
                        table.AddCell(new PdfPCell(new Phrase(reason != null && status == "Not Completed" ? $"Reason : {reason}" : "", mainFont))
                        {
                            Border = Rectangle.NO_BORDER
                        });

                        document.Add(table);

                        // Comment row if reason is Excused
                        if (reason == "Excused" && !string.IsNullOrEmpty(comment))
                        {
                            var commentTable = new PdfPTable(2) { WidthPercentage = 100 };
                            commentTable.SetWidths(new float[] { 60f, 40f });

                            // Empty left cell
                            commentTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });

                            // Comment in right cell
                            commentTable.AddCell(new PdfPCell(new Phrase($"Comment : {comment}", mainFont)) { Border = Rectangle.NO_BORDER });

                            document.Add(commentTable);
                        }

                        document.Add(Chunk.NEWLINE);
                    }



                    AddImmRow(
                            "Immunization",
                            dto.ImmunizationRecord.Status
                        );

                    if (dto.HepB == "NEEDED")
                    {
                        AddImmRow(
                            "HepB",
                            dto.ImmunizationRecord.HepBNeeded,
                            dto.ImmunizationRecord.HepBNeeded == "Not Completed" ? dto.ImmunizationRecord.HepBReason : null,
                            dto.ImmunizationRecord.HepBReason == "Excused" ? dto.ImmunizationRecord.HepBReasonExcusedComments : null
                        );
                    }

                    if (dto.Flu == "NEEDED")
                    {
                        AddImmRow(
                            "Flu",
                            dto.ImmunizationRecord.FluNeeded,
                            dto.ImmunizationRecord.FluNeeded == "Not Completed" ? dto.ImmunizationRecord.FluReason : null,
                            dto.ImmunizationRecord.FluReason == "Excused" ? dto.ImmunizationRecord.FluReasonExcusedComments : null
                        );
                    }

                    if (dto.Mmr == "NEEDED")
                    {
                        AddImmRow(
                            "MMR",
                            dto.ImmunizationRecord.MMRNeeded,
                            dto.ImmunizationRecord.MMRNeeded == "Not Completed" ? dto.ImmunizationRecord.MMRReason : null,
                            dto.ImmunizationRecord.MMRReason == "Excused" ? dto.ImmunizationRecord.MMRReasonExcusedComments : null
                        );
                    }

                    if (dto.HepA == "NEEDED")
                    {
                        AddImmRow(
                            "HepA",
                            dto.ImmunizationRecord.HepANeeded,
                            dto.ImmunizationRecord.HepANeeded == "Not Completed" ? dto.ImmunizationRecord.HepAReason : null,
                            dto.ImmunizationRecord.HepAReason == "Excused" ? dto.ImmunizationRecord.HepAReasonExcusedComments : null
                        );
                    }

                    if (dto.TetTdp == "NEEDED")
                    {
                        AddImmRow(
                            "Tet/Tdp",
                            dto.ImmunizationRecord.TetTdpNeeded,
                            dto.ImmunizationRecord.TetTdpNeeded == "Not Completed" ? dto.ImmunizationRecord.TetTdpReason : null,
                            dto.ImmunizationRecord.TetTdpReason == "Excused" ? dto.ImmunizationRecord.TetTdpReasonExcusedComments : null
                        );
                    }

                    if (dto.Varicella == "NEEDED")
                    {
                        AddImmRow(
                            "Varicella",
                            dto.ImmunizationRecord.VaricellaNeeded,
                            dto.ImmunizationRecord.VaricellaNeeded == "Not Completed" ? dto.ImmunizationRecord.VaricellaReason : null,
                            dto.ImmunizationRecord.VaricellaReason == "Excused" ? dto.ImmunizationRecord.VaricellaReasonExcusedComments : null
                        );
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




    }

}
