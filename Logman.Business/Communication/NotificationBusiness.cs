using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Logman.Common.Contracts;
using Logman.Common.Logging;
using Microsoft.Practices.Unity;

namespace Logman.Business.Communication
{
    public class NotificationBusiness : INotification
    {
        [Dependency]
        public IEmailProvider EmailProvider { get; set; }

        [Dependency]
        public ILogger Logger { get; set; }

        public async Task SendEmailAsync(string toAddress, string body)
        {
            var imagePaths = new Dictionary<string, string>();
            if (HttpContext.Current != null)
            {
                imagePaths.Add("myImageID", HttpContext.Current.Server.MapPath("~/Content/Images/LogManLogo.jpg"));
            }

            await EmailProvider.SendAsync("", toAddress, "Logman Alert!", body, imagePaths);
        }

        public async Task CallWebHookAsync(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    await client.GetAsync(url);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception);
                }
            }
        }
    }
}