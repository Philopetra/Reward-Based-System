using MailKit.Net.Smtp;
using MimeKit;

namespace RYT.Services.Emailing
{
    public class Emailing : IEmailService
    {
        private readonly IConfiguration _config;
        public Emailing(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<string> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var senderEmail = _config.GetSection("EmailSettings:sender").Value;
                var port = Convert.ToInt32(_config.GetSection("EmailSettings:port").Value);
                var host = _config.GetSection("EmailSettings:host").Value;
                var appPassword = _config.GetSection("EmailSettings:appPassword").Value;

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(senderEmail);
                email.To.Add(MailboxAddress.Parse(recipientEmail));
                email.Subject = subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    smtp.CheckCertificateRevocation = true;
                    await smtp.ConnectAsync(host, port, true);
                    await smtp.AuthenticateAsync(senderEmail, appPassword);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

            return "";
        }
    }
}
