using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using PlatformAPI.Core.Helpers;
using PlatformAPI.Core.Interfaces;

namespace PlatformAPI.Core.Services
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailSettings;
        public MailingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject
            };
            email.To.Add(MailboxAddress.Parse(mailTo));

            var builder = new BodyBuilder();
            if(attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if(file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        builder.Attachments.Add(file.FileName, fileBytes,ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = body;
            email.Body=builder.ToMessageBody();
            email.From.Add(MailboxAddress.Parse(_mailSettings.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email,_mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
