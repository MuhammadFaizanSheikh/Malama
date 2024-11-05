using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class EmailSender : IEmailSender
    {
        //private readonly IConfiguration _configuration;
        //private readonly SmtpClient _smtpClient;
        //private readonly string _fromEmail;

        //public EmailSender(IConfiguration configuration)
        //{
        //    var emailSettings = configuration.GetSection("Email:Smtp");
        //    _smtpClient = new SmtpClient(emailSettings["Host"])
        //    {
        //        Port = int.Parse(emailSettings["Port"]),
        //        Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
        //        EnableSsl = bool.Parse(emailSettings["UseSsl"]) // Ensure SSL is enabled
        //    };
        //    _fromEmail = emailSettings["From"];
        //}

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
            emailMessage.From.Add(new MailboxAddress("Muhammad", "muhamadfaizansheikh@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; // Disable SSL validation for testing (not recommended for production)
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("muhamadfaizansheikh@gmail.com", "Iloveyou123456");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
