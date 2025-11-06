namespace EnrollmentSystem.Services
{
    public interface IEmailCampaignService
    {
        Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody);
        Task<bool> SendNewsletterAsync(string subject, string content, List<string> recipients);
        Task<bool> SendPromotionalEmailAsync(string promoCode, List<string> recipients);
    }

    public class EmailCampaignService : IEmailCampaignService
    {
        private readonly ILogger<EmailCampaignService> _logger;
        private readonly IConfiguration _configuration;

        public EmailCampaignService(ILogger<EmailCampaignService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody)
        {
            // TODO: Integrate with SendGrid, Mailchimp, or similar
            _logger.LogInformation($"Sending bulk email to {recipients.Count} recipients: {subject}");

            foreach (var recipient in recipients)
            {
                // Simulate sending
                await Task.Delay(10);
            }

            return true;
        }

        public async Task<bool> SendNewsletterAsync(string subject, string content, List<string> recipients)
        {
            var htmlBody = $@"
                <html>
                <body>
                    <h1>{subject}</h1>
                    <div>{content}</div>
                    <hr>
                    <p><small>You are receiving this email because you are subscribed to our newsletter.</small></p>
                </body>
                </html>";

            return await SendBulkEmailAsync(recipients, subject, htmlBody);
        }

        public async Task<bool> SendPromotionalEmailAsync(string promoCode, List<string> recipients)
        {
            var subject = "Special Offer - Limited Time Only!";
            var htmlBody = $@"
                <html>
                <body>
                    <h1>Exclusive Offer Just for You!</h1>
                    <p>Use code <strong>{promoCode}</strong> to get your discount.</p>
                    <p>This offer is valid for a limited time only.</p>
                </body>
                </html>";

            return await SendBulkEmailAsync(recipients, subject, htmlBody);
        }
    }
}
