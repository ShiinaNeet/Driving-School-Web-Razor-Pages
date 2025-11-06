using EnrollmentSystem.Models;

namespace EnrollmentSystem.Services
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificatePdfAsync(Certificate certificate);
        Task<string> GenerateVerificationCodeAsync(Certificate certificate);
        Task<Certificate?> VerifyCertificateAsync(string verificationCode);
        Task<bool> RevokeCertificateAsync(int certificateId, string reason);
    }
}
