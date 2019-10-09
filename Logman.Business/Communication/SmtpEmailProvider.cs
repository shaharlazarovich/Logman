using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using Logman.Common.Contracts;

namespace Logman.Business.Communication
{
    public class SmtpEmailProvider : IEmailProvider
    {
        public async Task SendAsync(string from, string to, string subject, string body,
            Dictionary<string, string> imagePaths = null)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            var client = new SmtpClient
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password)
            };


            var message = new MailMessage();
            if (imagePaths != null && imagePaths.Any())
            {
                foreach (var item in imagePaths)
                {
                    message.Attachments.Add(new Attachment(item.Value) {ContentId = item.Key});
                }
            }

            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;
            await Task.Run(() => client.SendAsync(message, null));
        }
    }
}