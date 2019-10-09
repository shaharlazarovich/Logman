using System;
using System.Web;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Web.App_Start;
using Logman.Web.Models.Shared;
using Microsoft.Practices.Unity;

namespace Logman.Web
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            UnityConfig.GetConfiguredContainer();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ScheduledTasksConfig.ConfigureAlerts();
        }

        private void Session_Start(object sender, EventArgs e)
        {
            var viewStartViewModel = new ViewStartViewModel();
            var session = UnityConfig.GetConfiguredContainer().Resolve<ISessionProvider>();
            session[Constants.LayoutViewModelName] = viewStartViewModel;
        }
    }
}