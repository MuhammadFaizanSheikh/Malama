using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ITreatmentConsentPdfGenerator
    {
        byte[] GenerateQuestionnairePdf(ServiceMembersChild serviceMember, DentalQuestionnaire questionnaire);

        byte[] GenerateDentalTreatmentConsentPdf(
            ServiceMembersChild serviceMember,
            TreatmentConsentDentalTreatmentFormDto form,
            byte[]? signatureBytes);

        byte[] GenerateOralSurgeryConsentPdf(
            ServiceMembersChild serviceMember,
            TreatmentConsentOralSurgeryFormDto form,
            byte[]? signatureBytes);
    }
}
