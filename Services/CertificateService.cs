using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EnrollmentSystem.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CertificateService> _logger;

        public CertificateService(ApplicationDbContext context, ILogger<CertificateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<byte[]> GenerateCertificatePdfAsync(Certificate certificate)
        {
            // Load certificate with related data
            var cert = await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .Include(c => c.Enrollment)
                .FirstOrDefaultAsync(c => c.Id == certificate.Id);

            if (cert == null)
            {
                throw new ArgumentException("Certificate not found");
            }

            // Generate simple HTML-based certificate
            var html = GenerateCertificateHtml(cert);

            // Convert HTML to PDF bytes (simplified - in production, use a library like SelectPdf, iTextSharp, or PuppeteerSharp)
            var pdfBytes = ConvertHtmlToPdf(html);

            return pdfBytes;
        }

        public async Task<string> GenerateVerificationCodeAsync(Certificate certificate)
        {
            // Generate unique verification code
            var data = $"{certificate.CertificateNumber}:{certificate.StudentId}:{certificate.IssueDate:yyyyMMdd}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(data));
            var verificationCode = Convert.ToBase64String(hash).Replace("+", "").Replace("/", "").Substring(0, 16);

            certificate.VerificationCode = verificationCode;
            await _context.SaveChangesAsync();

            return verificationCode;
        }

        public async Task<Certificate?> VerifyCertificateAsync(string verificationCode)
        {
            return await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.VerificationCode == verificationCode && c.Status == CertificateStatus.Issued);
        }

        public async Task<bool> RevokeCertificateAsync(int certificateId, string reason)
        {
            var certificate = await _context.Certificates.FindAsync(certificateId);
            if (certificate == null)
            {
                return false;
            }

            certificate.Status = CertificateStatus.Revoked;
            certificate.Notes = $"Revoked: {reason}\n{certificate.Notes}";
            certificate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateCertificateHtml(Certificate certificate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{
            font-family: 'Georgia', serif;
            text-align: center;
            padding: 50px;
            background: #f8f9fa;
        }}
        .certificate {{
            max-width: 800px;
            margin: 0 auto;
            padding: 60px;
            background: white;
            border: 15px solid #1a5490;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }}
        h1 {{
            font-size: 48px;
            color: #1a5490;
            margin-bottom: 10px;
        }}
        h2 {{
            font-size: 28px;
            color: #333;
            margin: 30px 0;
        }}
        .student-name {{
            font-size: 36px;
            color: #1a5490;
            font-weight: bold;
            margin: 30px 0;
            text-decoration: underline;
        }}
        .course-name {{
            font-size: 24px;
            color: #555;
            margin: 20px 0;
        }}
        .date {{
            font-size: 18px;
            color: #777;
            margin: 30px 0;
        }}
        .signature {{
            margin-top: 60px;
            font-size: 16px;
        }}
        .cert-number {{
            font-size: 14px;
            color: #999;
            margin-top: 40px;
        }}
    </style>
</head>
<body>
    <div class='certificate'>
        <h1>CERTIFICATE OF COMPLETION</h1>
        <h2>This certifies that</h2>
        <div class='student-name'>{certificate.Student.FullName}</div>
        <h2>has successfully completed</h2>
        <div class='course-name'>{certificate.Title}</div>
        {(certificate.Course != null ? $"<p>{certificate.Course.Description}</p>" : "")}
        <div class='date'>Date of Completion: {certificate.IssueDate:MMMM dd, yyyy}</div>
        {(certificate.ExpiryDate.HasValue ? $"<div class='date'>Valid Until: {certificate.ExpiryDate.Value:MMMM dd, yyyy}</div>" : "")}
        <div class='signature'>
            <p>_________________________________</p>
            <p>{certificate.IssuedBy ?? "Authorized Signature"}</p>
        </div>
        <div class='cert-number'>
            Certificate No: {certificate.CertificateNumber}<br>
            Verification Code: {certificate.VerificationCode}
        </div>
    </div>
</body>
</html>";
        }

        private byte[] ConvertHtmlToPdf(string html)
        {
            // Simplified implementation - returns HTML as bytes
            // In production, use SelectPdf, iTextSharp, PuppeteerSharp, or similar library
            // For now, just return UTF-8 encoded HTML (which can be saved as .html instead of .pdf)
            return Encoding.UTF8.GetBytes(html);
        }
    }
}
