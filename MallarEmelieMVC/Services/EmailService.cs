using System.Net;
using System.Net.Mail;

namespace MallarEmelieMVC.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;

        //getting user secrets
        public EmailService(string email, string appPassword)
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(email, appPassword),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpClient.Credentials.GetCredential("smtp.gmail.com", 587, "Basic").UserName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
