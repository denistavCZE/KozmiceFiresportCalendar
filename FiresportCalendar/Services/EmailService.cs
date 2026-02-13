using Azure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Azure.Communication.Email;
using Microsoft.Extensions.Caching.Memory;
using FiresportCalendar.Exceptions;
namespace FiresportCalendar.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IMemoryCache _cache;
        private readonly string _connectionString;
        private readonly string _senderAddress;
        private readonly int _dailyEmailLimit;
        
        public EmailService(IConfiguration configuration, IMemoryCache cache) {
            _connectionString = configuration.GetConnectionString("EmailServiceConnection");
            _senderAddress = configuration["EmailSenderAddress"];
            _dailyEmailLimit = int.Parse(configuration["DailyEmailLimit"] ?? "0");
            _cache = cache;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var lastEmailDate =  _cache.GetOrCreate("LastEmailDate", entry => DateTime.UtcNow.Date);
            var emailCountToday = _cache.GetOrCreate("EmailCount", entry => 0);

            if (lastEmailDate != DateTime.UtcNow.Date)
            {
                _cache.Set("EmailCount", 0);
                _cache.Set("LastEmailDate", DateTime.UtcNow.Date);
            }

            if (emailCountToday < _dailyEmailLimit)
            {
                _cache.Set("EmailCount", emailCountToday + 1);

                var emailClient = new EmailClient(_connectionString);


                var emailMessage = new EmailMessage(
                    senderAddress: _senderAddress,
                    content: new EmailContent(subject)
                    {
                        Html = htmlMessage
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(email) }));


                EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                    WaitUntil.Completed,
                    emailMessage);
            }
            else throw new DailyEmailLimitException();
        }
    }
}
