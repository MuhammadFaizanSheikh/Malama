using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        //private readonly SmtpClient _smtpClient;
        private readonly string userName;
        private readonly string password;
        private readonly string host;
        private readonly string fromEmail;
        private readonly int port;

        public EmailSender(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("Email:Smtp");
            userName = emailSettings["Username"];
            password = emailSettings["Password"];
            host = emailSettings["Host"];
            fromEmail = emailSettings["From"];
            port = Convert.ToInt32(emailSettings["Port"]);
            //_smtpClient = new SmtpClient(emailSettings["Host"])
            //{
            //    Port = int.Parse(emailSettings["Port"]),
            //    Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
            //    EnableSsl = bool.Parse(emailSettings["UseSsl"]) // Ensure SSL is enabled
            //};
            //_fromEmail = emailSettings["From"];
        }

        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress(_fromEmail),
        //        Subject = subject,
        //        Body = message,
        //        IsBodyHtml = true,
        //    };
        //    mailMessage.To.Add(email);

        //    await _smtpClient.SendMailAsync(mailMessage);
        //}

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(userName, fromEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Disable SSL validation for testing (not recommended for production)
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
