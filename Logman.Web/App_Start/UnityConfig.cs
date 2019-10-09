using System;
using Logman.Business.Account;
using Logman.Business.Applications;
using Logman.Business.Caching;
using Logman.Business.Communication;
using Logman.Business.Engines;
using Logman.Business.Events;
using Logman.Business.SessionManagement;
using Logman.Common.Contracts;
using Logman.Common.Data;
using Logman.Common.Logging;
using Logman.Data.SqlServer.Facade;
using Microsoft.Practices.Unity;

namespace Logman.Web.App_Start
{
    /// <summary>
    ///     Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container

        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        ///     Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            container
                .RegisterType<ILogger, Nlogger>()
                .RegisterType<ICacheProvider, AspNetCaheProvider>()
                .RegisterType<ISessionProvider, AspnetSessionProvider>()
                .RegisterType<IDataAccessLayer, SqlDataAccessLayer>()
                .RegisterType<IEventBusiness, EventBusiness>()
                .RegisterType<IApplicationBusiness, ApplicationBusiness>()
                .RegisterType<IAccountBusiness, AccountBusiness>()
                .RegisterType<IEmailProvider, SmtpEmailProvider>()
                .RegisterType<IAlerEngine, AlertEngine>()
                .RegisterType<INotification, NotificationBusiness>();

            container.Resolve<IDataAccessLayer>().Initialize();
        }
    }
}