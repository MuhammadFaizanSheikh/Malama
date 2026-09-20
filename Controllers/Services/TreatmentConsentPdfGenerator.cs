using ExcelFilesCompiler.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Malama.Models;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class TreatmentConsentPdfGenerator : ITreatmentConsentPdfGenerator
    {
        private readonly Font _titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 16);
        private readonly Font _headingFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);
        private readonly Font _sectionFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 11);
        private readonly Font _bodyFont = FontFactory.GetFont(FontFactory.TIMES, 10);
        private readonly Font _smallFont = FontFactory.GetFont(FontFactory.TIMES, 9);
        private readonly Font _boldSmallFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);

        public byte[] GenerateQuestionnairePdf(ServiceMembersChild serviceMember, DentalQuestionnaire questionnaire)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.LETTER, 42f, 42f, 42f, 42f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            document.Add(new Paragraph("DA5570 — Dental Questionnaire", _titleFont) { SpacingAfter = 4f });
            document.Add(new Paragraph("Completed by Service Member", _smallFont) { SpacingAfter = 10f });
            AddPatientHeader(document, serviceMember);

            AddQa(document, "1. Have you been under a Health Care Provider's care in the last 2 years?",
                questionnaire.HealthcareProviderCareLast2Years);
            AddQa(document, "2. Have you had any serious illness, operation, or hospitalization in the past?",
                questionnaire.SeriousIllnessOperationHospitalization,
                questionnaire.SeriousIllnessOperationHospitalizationDetail);
            AddQa(document, "3. Do you have allergies to medication, food, a vaccine component, or latex?",
                questionnaire.MedicationFoodAllergy,
                questionnaire.MedicationFoodAllergyDetail);
            AddQa(document, "4. Are you presently taking any drugs or medications (to include birth control pills)?",
                questionnaire.TakingMedications,
                questionnaire.TakingMedicationsDetail);
            AddQa(document, "5. Have you ever had hepatitis or jaundice?",
                questionnaire.HepatitisOrJaundice);
            AddQa(document, "6. Has there been a change in your health in the last two years?",
                questionnaire.HealthChangeLastTwoYears);

            AddQa(document, "7. Do you use tobacco or vape?", questionnaire.UseTobaccoOrVape);
            if (IsYes(questionnaire.UseTobaccoOrVape))
            {
                var tobacco = DentalQuestionnaireService.ParseTobaccoDetails(questionnaire.TobaccoUseDetailsJson);
                foreach (var type in DentalXRayQuestionnaire.TobaccoTypes)
                {
                    var detail = tobacco.FirstOrDefault(t =>
                        string.Equals(t.Type, type, StringComparison.OrdinalIgnoreCase));
                    var used = string.IsNullOrWhiteSpace(detail?.Used) ? "No" : detail!.Used!.Trim();
                    var frequency = IsYes(used)
                        ? $" (Times/day: {Blank(detail?.TimesPerDay)}; Times/week: {Blank(detail?.TimesPerWeek)})"
                        : string.Empty;
                    document.Add(new Paragraph($"    • {type}: {used}{frequency}", _smallFont) { SpacingAfter = 2f });
                }
            }

            AddQa(document, "8. Do you drink alcoholic beverages?",
                questionnaire.DrinkAlcoholicBeverages,
                questionnaire.AlcoholicBeveragesFrequencyQuantity);
            AddQa(document, "9. Have you ever been sick because of dental treatments?",
                questionnaire.SickFromDentalTreatment);
            AddQa(document, "10. Are you a bleeder or have you had excessive bleeding following dental treatment?",
                questionnaire.BleederOrExcessiveBleeding);
            AddQa(document, "11. Do you get short of breath after climbing one flight of stairs?",
                questionnaire.ShortOfBreathOneFlightStairs);

            if (!string.IsNullOrWhiteSpace(questionnaire.AreYouPregnant))
            {
                AddQa(document, "Are you pregnant?", questionnaire.AreYouPregnant, questionnaire.PregnancyApproval);
            }

            var conditions = DentalQuestionnaireService.ParseHealthConditionsJson(questionnaire.ApplicableHealthConditionsJson)
                .Where(c => c.IsSelected && !string.IsNullOrWhiteSpace(c.Condition))
                .ToList();
            document.Add(new Paragraph("Applicable health conditions", _sectionFont) { SpacingBefore = 8f, SpacingAfter = 4f });
            if (conditions.Count == 0)
            {
                document.Add(new Paragraph("None selected", _bodyFont) { SpacingAfter = 4f });
            }
            else
            {
                foreach (var condition in conditions)
                {
                    var detail = string.IsNullOrWhiteSpace(condition.Detail) ? string.Empty : $" — {condition.Detail.Trim()}";
                    document.Add(new Paragraph($"• {condition.Condition}{detail}", _bodyFont) { SpacingAfter = 2f });
                }
            }

            document.Close();
            return ms.ToArray();
        }

        public byte[] GenerateDentalTreatmentConsentPdf(
            ServiceMembersChild serviceMember,
            TreatmentConsentDentalTreatmentFormDto form,
            byte[]? signatureBytes)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.LETTER, 42f, 42f, 42f, 42f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            var dentistName = string.IsNullOrWhiteSpace(form.DentistName) ? "____________________" : form.DentistName.Trim();
            var patientName = string.IsNullOrWhiteSpace(serviceMember.FullName) ? "____________________" : serviceMember.FullName.Trim();
            var dob = Blank(serviceMember.Dob);
            var consentDate = DateTime.Now.ToString("MM/dd/yyyy");

            document.Add(new Paragraph("CONSENT FOR DENTAL TREATMENT", _titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            });

            document.Add(new Paragraph($"Patient's Name: {patientName}     Birth Date: {dob}", _bodyFont) { SpacingAfter = 6f });
            document.Add(new Paragraph(
                "Please read and initial the items checked below. Then read and sign the section at the bottom of the form.",
                _bodyFont)
            { SpacingAfter = 8f });

            AddConsentItem(document, form.Item1Mark, "1. TREATMENT TO BE PERFORMED", form.Item1Initials,
                "I understand that I am having the following work done:",
                BuildTreatmentList(form));
            if (form.OtherTreatment && !string.IsNullOrWhiteSpace(form.OtherTreatmentText))
            {
                document.Add(new Paragraph($"Other treatment: {form.OtherTreatmentText.Trim()}", _bodyFont) { SpacingAfter = 4f });
            }

            AddConsentItem(document, form.Item2Mark, "2. DRUGS AND MEDICATION", form.Item2Initials,
                "I understand that antibiotics and analgesics and other medications can cause allergic reactions causing redness and swelling of tissues, pain, itching, vomiting, and/or anaphylactic shock (severe allergic reaction).");

            AddConsentItem(document, form.Item3Mark, "3. REMOVAL OF TEETH", form.Item3Initials,
                $"Alternatives to removal have been explained to me (root canal therapy, crowns, periodontal surgery, etc.) and I authorize Dr. {dentistName} to remove the following teeth: {Blank(form.RemovalTeeth)}. I understand removing teeth does not always remove all the infection, if present, and it may be necessary to have further treatment.");

            AddConsentItem(document, form.Item4Mark, "4. ENDODONTIC TREATMENT", form.Item4Initials,
                "I realize there is no guarantee that the root canal treatment will save my tooth, that complications can occur from the treatment, and occasionally metal objects are cemented in the tooth or extended through the root, which does not necessarily affect the success of the treatment. I understand that occasionally additional surgical procedures may be necessary following root canal treatment (apicoectomy).");

            AddConsentItem(document, form.Item5Mark, "5. SECTIONING OF BRIDGE/REMOVAL OF CROWN", form.Item5Initials,
                "I understand that if a bridge must be sectioned (cut in half) or a crown removed in order to complete my treatment this does not obligate DAWSON to provide a new crown or bridge.");

            AddConsentItem(document, form.Item6Mark, "6. OTHER", form.Item6Initials,
                string.IsNullOrWhiteSpace(form.OtherSituation)
                    ? "Please describe any situation not listed above that requires consent."
                    : form.OtherSituation.Trim());

            document.Add(new Paragraph(
                "I understand that during treatment it may be necessary to change or add procedures because of conditions found while working on teeth that were not discovered during the examination. I also understand that dentistry is not an exact science and that reputable practitioners cannot guarantee results. I acknowledge that no guarantee or assurance has been made to me by anyone regarding dental treatment that I have requested and authorized for myself.",
                _bodyFont)
            { SpacingBefore = 8f, SpacingAfter = 6f });

            document.Add(new Paragraph(
                "In the event of an emergency, contact your State Dental Case Manager and/or dial 911",
                _boldSmallFont)
            { SpacingAfter = 10f });

            document.Add(new Paragraph("Signature of Patient, Parent, Guardian, or Personal Representative", _sectionFont)
            {
                SpacingAfter = 4f
            });
            AddSignatureImage(document, signatureBytes);
            document.Add(new Paragraph($"Date: {consentDate}", _bodyFont) { SpacingBefore = 6f });

            document.Close();
            return ms.ToArray();
        }

        public byte[] GenerateOralSurgeryConsentPdf(
            ServiceMembersChild serviceMember,
            TreatmentConsentOralSurgeryFormDto form,
            byte[]? signatureBytes)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.LETTER, 42f, 42f, 42f, 42f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            var dentistName = string.IsNullOrWhiteSpace(form.DentistName) ? "____________________" : form.DentistName.Trim();
            var patientName = string.IsNullOrWhiteSpace(serviceMember.FullName) ? "____________________" : serviceMember.FullName.Trim();
            var consentDate = DateTime.Now.ToString("MM/dd/yyyy");

            document.Add(new Paragraph("ORAL SURGERY CONSENT FORM", _titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            });

            document.Add(new Paragraph(
                "You have a right to be informed about your diagnosis and planned surgery so that you may decide whether to undergo a procedure after knowing the risks and hazards. This is to make you better informed so you may give your informed consent to the procedure.",
                _bodyFont)
            { SpacingAfter = 8f });

            document.Add(new Paragraph("Possible conditions of:", _sectionFont) { SpacingAfter = 4f });

            AddOsSection(document, "1. ALL SURGERIES", new[]
            {
                "Soreness, swelling, bruising, and restricted mouth opening during healing, sometimes related to swelling and muscle soreness or to stress on the jaw joints (TMJ), especially when TMJ problems already exist.",
                "Bleeding, usually controllable, but maybe prolonged and require additional care.",
                "Drug reactions or allergies.",
                "Infection, possibly requiring additional care and/or surgeries."
            });
            AddOsSection(document, "2. ALL TOOTH EXTRACTIONS", new[]
            {
                "Dry socket, discomfort occurring a few days after extraction requires further care.",
                "Damage to adjacent teeth or fillings.",
                "Sharp ridges or bone splinters may require additional surgery to smooth area.",
                "Portions of tooth remaining, sometimes fine root tips break off and may be deliberately left in place to avoid doing damage to nearby vital structures such as nerves or the sinus."
            });
            AddOsSection(document, "3. LOWER TEETH", new[]
            {
                "Numbness: due to the proximity of roots in the nerve especially wisdom teeth. It is possible to injure the nerve during the removal of the tooth. The lip, chin, gums or tongue could feel numb resembling local anesthetic injections, and this could remain for days, weeks, or very rarely, permanently."
            });
            AddOsSection(document, "4. UPPER TEETH", new[]
            {
                "Sinus involvement: due to the closeness of the roots of upper back teeth to the sinus or from the root tip being displaced to the sinus, a possible sinus infection or sinus opening may result, which may require a medication and/or later surgery to correct."
            });
            AddOsSection(document, "5. ANESTHESIA", new[]
            {
                "Local anesthesia: certain possible risks that, although rare, could include pain, swelling, bruising, infection, nerve damage, and unexpected allergic reactions which could result in heart attack, stroke, brain damage and/or death."
            });

            document.Add(new Paragraph(
                $"I, {patientName}, hereby authorize Dr. {dentistName} to perform the following procedure:",
                _bodyFont)
            { SpacingBefore = 8f, SpacingAfter = 4f });
            document.Add(new Paragraph(
                string.IsNullOrWhiteSpace(form.ProcedureText) ? "(No procedure described)" : form.ProcedureText.Trim(),
                _bodyFont)
            { SpacingAfter = 6f });

            document.Add(new Paragraph(
                "and to administer anesthesia. I understand that the doctor may discover other or different conditions that may require additional or different procedures than those planned. I authorize him/her to perform such other procedures, as he/she deems necessary in his/her professional judgment in order to complete my surgery.",
                _bodyFont)
            { SpacingAfter = 6f });

            document.Add(new Paragraph(
                $"I have read and discussed the preceding with Dr. {dentistName} and believe I have been given sufficient information to give my consent to the planned surgery.",
                _bodyFont)
            { SpacingAfter = 10f });

            document.Add(new Paragraph("Patient (or legal guardian) Signature", _sectionFont) { SpacingAfter = 4f });
            AddSignatureImage(document, signatureBytes);
            document.Add(new Paragraph($"Date (MM/DD/YYYY): {consentDate}", _bodyFont) { SpacingBefore = 6f });

            document.Close();
            return ms.ToArray();
        }

        private void AddPatientHeader(Document document, ServiceMembersChild serviceMember)
        {
            var table = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 10f };
            table.SetWidths(new float[] { 1f, 2f });
            AddHeaderCell(table, "Patient Name", serviceMember.FullName);
            AddHeaderCell(table, "Birth Date", serviceMember.Dob);
            AddHeaderCell(table, "Barcode", serviceMember.Barcode);
            AddHeaderCell(table, "DoD ID / Last 4", $"{Blank(serviceMember.DodId)} / {Blank(serviceMember.Last4)}");
            document.Add(table);
        }

        private void AddHeaderCell(PdfPTable table, string label, string? value)
        {
            table.AddCell(new PdfPCell(new Phrase(label, _boldSmallFont))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 3f
            });
            table.AddCell(new PdfPCell(new Phrase(Blank(value), _bodyFont))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 3f
            });
        }

        private void AddQa(Document document, string question, string? answer, string? detail = null)
        {
            document.Add(new Paragraph(question, _sectionFont) { SpacingBefore = 6f, SpacingAfter = 2f });
            document.Add(new Paragraph($"Answer: {Blank(answer)}", _bodyFont) { SpacingAfter = 2f });
            if (IsYes(answer) && !string.IsNullOrWhiteSpace(detail))
            {
                document.Add(new Paragraph($"Detail: {detail.Trim()}", _smallFont) { SpacingAfter = 2f });
            }
        }

        private void AddConsentItem(
            Document document,
            bool marked,
            string title,
            string? initials,
            string body,
            string? treatments = null)
        {
            var mark = marked ? "[X]" : "[ ]";
            document.Add(new Paragraph($"{mark} {title}", _headingFont) { SpacingBefore = 8f, SpacingAfter = 3f });
            document.Add(new Paragraph(body, _bodyFont) { SpacingAfter = 2f });
            if (!string.IsNullOrWhiteSpace(treatments))
            {
                document.Add(new Paragraph(treatments, _bodyFont) { SpacingAfter = 2f });
            }

            document.Add(new Paragraph($"(Initials {Blank(initials)})", _boldSmallFont) { SpacingAfter = 4f });
        }

        private static string BuildTreatmentList(TreatmentConsentDentalTreatmentFormDto form)
        {
            var items = new List<string>();
            if (form.Fillings) items.Add("Fillings");
            if (form.Crowns) items.Add("Stainless Steel Crowns");
            if (form.Extractions) items.Add("Extracted");
            if (form.Impacted) items.Add("Impacted Teeth Removed");
            if (form.RootCanal) items.Add("Root Canal(s)");
            if (form.Fmd) items.Add("FMD");
            if (form.OtherTreatment) items.Add("Other");
            return items.Count == 0 ? "None selected" : string.Join(", ", items);
        }

        private void AddOsSection(Document document, string title, IEnumerable<string> bullets)
        {
            document.Add(new Paragraph(title, _sectionFont) { SpacingBefore = 6f, SpacingAfter = 3f });
            foreach (var bullet in bullets)
            {
                document.Add(new Paragraph($"• {bullet}", _smallFont) { SpacingAfter = 2f });
            }
        }

        private void AddSignatureImage(Document document, byte[]? signatureBytes)
        {
            if (signatureBytes == null || signatureBytes.Length == 0)
            {
                document.Add(new Paragraph("[Signature not on file]", _smallFont) { SpacingAfter = 4f });
                return;
            }

            try
            {
                var image = Image.GetInstance(signatureBytes);
                image.ScaleToFit(280f, 90f);
                document.Add(image);
            }
            catch
            {
                document.Add(new Paragraph("[Signature could not be rendered]", _smallFont) { SpacingAfter = 4f });
            }
        }

        private static bool IsYes(string? value) =>
            string.Equals(value?.Trim(), "Yes", StringComparison.OrdinalIgnoreCase);

        private static string Blank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "—" : value.Trim();
    }
}
