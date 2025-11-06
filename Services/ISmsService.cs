namespace EnrollmentSystem.Services
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message);
        Task<bool> SendLessonReminderAsync(string studentPhone, string lessonDetails, DateTime lessonTime);
        Task<bool> SendPaymentReminderAsync(string studentPhone, decimal amount);
    }

    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            // TODO: Integrate with Twilio
            // var accountSid = _configuration["Twilio:AccountSid"];
            // var authToken = _configuration["Twilio:AuthToken"];
            // var fromNumber = _configuration["Twilio:FromNumber"];

            _logger.LogInformation($"SMS to {phoneNumber}: {message}");

            // Simulated success
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> SendBulkSmsAsync(List<string> phoneNumbers, string message)
        {
            foreach (var phone in phoneNumbers)
            {
                await SendSmsAsync(phone, message);
            }
            return true;
        }

        public async Task<bool> SendLessonReminderAsync(string studentPhone, string lessonDetails, DateTime lessonTime)
        {
            var message = $"Reminder: Your {lessonDetails} is scheduled for {lessonTime:MMM dd, yyyy h:mm tt}. See you there!";
            return await SendSmsAsync(studentPhone, message);
        }

        public async Task<bool> SendPaymentReminderAsync(string studentPhone, decimal amount)
        {
            var message = $"Payment reminder: You have an outstanding balance of ${amount:N2}. Please visit our website to make a payment.";
            return await SendSmsAsync(studentPhone, message);
        }
    }
}
