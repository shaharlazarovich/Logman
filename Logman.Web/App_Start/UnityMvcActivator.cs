using System.Linq;
using System.Web.Mvc;
using Logman.Web.App_Start;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (UnityWebActivator), "Start")]

namespace Logman.Web.App_Start
{
    public static class UnityWebActivator
    {
        public static void Start()
        {
            IUnityContainer container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}