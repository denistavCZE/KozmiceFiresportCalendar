using Azure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Azure.Communication.Email;
namespace FiresportCalendar.Services
{
    public class EmailService : IEmailSender
    {
        string _connectionString;
        string _senderAddress;
        public EmailService(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("EmailServiceConnection");
            _senderAddress = configuration["EmailSenderAddress"];
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

         

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
    }
}
