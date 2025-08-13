namespace SoldierTrack.Services.Email
{
    using System.Net;
    using System.Net.Mail;

    using Microsoft.Extensions.Options;
    using Models;
    
    public class EmailService(
        IOptions<SmtpSettings> smtpSettings) : IEmailService
    {
        private readonly SmtpSettings smtpSettings = smtpSettings.Value;

        public async Task SendEmail(
            string to,
            string subject,
            string body)
        {
            using var mail = new MailMessage();

            mail.From = new MailAddress(this.smtpSettings.Username);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using var smtp = new SmtpClient(
                this.smtpSettings.Host,
                this.smtpSettings.Port)
            {
                Credentials = new NetworkCredential(
                    this.smtpSettings.Username,
                    this.smtpSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
