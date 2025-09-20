using System.Net.Mail;

namespace AllInOneProject.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new System.Net.Mail.SmtpClient(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]));
            client.Credentials = new System.Net.NetworkCredential(
                _configuration["Email:Username"],
                _configuration["Email:Password"]
            );
            client.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new MailAddress(_configuration["Email:FromEmail"], _configuration["Email:FromName"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
