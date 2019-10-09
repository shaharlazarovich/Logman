using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Web.App_Start;
using Logman.Web.Models.Shared;
using Microsoft.Practices.Unity;

namespace Logman.Web.Code.Classes
{
    public static class Util
    {
        public static ViewStartViewModel GetLayoutViewModel()
        {
            var sessionProvider = UnityConfig.GetConfiguredContainer().Resolve<ISessionProvider>();
            var result = sessionProvider[Constants.LayoutViewModelName] as ViewStartViewModel;
            if (result == null)
            {
                sessionProvider[Constants.LayoutViewModelName] = new ViewStartViewModel();
            }
            return result;
        }
    }
}