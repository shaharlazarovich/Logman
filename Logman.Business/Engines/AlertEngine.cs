using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Logman.Common.Contracts;
using Logman.Common.DomainObjects;
using Logman.Common.Logging;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Logman.Business.Engines
{
    public class AlertEngine : IAlerEngine
    {
        public static bool IsInProgress;
        private readonly ConcurrentBag<Alert> _alerts;

        public AlertEngine()
        {
            _alerts = new ConcurrentBag<Alert>();
        }

        [Dependency]
        public IApplicationBusiness ApplicationBusiness { get; set; }


        [Dependency]
        public INotification NotificationBusiness { get; set; }

        [Dependency]
        public ILogger Logger { get; set; }

        public async Task LoadAlertsAsync()
        {
            List<Application> allApps = await ApplicationBusiness.FindAllAsync();
            allApps.ForEach(async app =>
            {
                List<Alert> alertsOfApp = await ApplicationBusiness.GetAlertsOfApp(app.Id);
                alertsOfApp.ForEach(alert => _alerts.Add(alert));
            });
        }

        public async Task ProcessAlertsAsync()
        {
            if (!IsInProgress)
            {
                var listOfAlerts = new Dictionary<long, DateTime>();
                IsInProgress = true;

                try
                {
                    _alerts.AsParallel().ForEach(async alert =>
                    {
                        int hourValue = GetHourPeriod(alert);
                        double hourGap = DateTime.UtcNow.Subtract(alert.LastExecutionTime).TotalHours;
                        if (hourGap >= hourValue)
                        {
                            await SendAlertFor(alert);
                            listOfAlerts.Add(alert.Id, DateTime.UtcNow);
                        }
                    });

                    await UpdateLastExecTimes(listOfAlerts);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
                finally
                {
                    IsInProgress = false;
                }
            }
        }

        private async Task UpdateLastExecTimes(Dictionary<long, DateTime> listOfAlerts)
        {
            await ApplicationBusiness.UpdateAlertExecTimes(listOfAlerts);
        }

        private async Task SendAlertFor(Alert alert)
        {
            if (alert.TypeOfNotification == NotificationType.WebHook)
            {
                await NotificationBusiness.CallWebHookAsync(alert.Target);
            }
            else
            {
                var eventTypes = new List<string>();
                if ((alert.EventLevelValue & (int) EventLevel.Fatal) == (int) EventLevel.Fatal)
                {
                    eventTypes.Add("Fatal Errors");
                }

                if ((alert.EventLevelValue & (int) EventLevel.Error) == (int) EventLevel.Error)
                {
                    eventTypes.Add("Errors");
                }

                if ((alert.EventLevelValue & (int) EventLevel.Warning) == (int) EventLevel.Warning)
                {
                    eventTypes.Add("Warnings");
                }

                Application app = await ApplicationBusiness.GetByIdAsync(alert.AppId);
                string body =
                    "Dear {0}<br/>, You are receiving this alert because the number of received {1} events from application {2} has exceeded the configured threshold which is {3}.";

                try
                {

                    var assembly = Assembly.GetExecutingAssembly();
                    const string resourceName = "Logman.Business.Resources.Alert.html";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                body = reader.ReadToEnd();
                            }
                }
                catch
                {
                    // Exceptions are ignored because in case of an error, we use the plain text.
                }
                
                body = string.Format(body, alert.Target, string.Join(" and ", eventTypes), app.AppName, alert.Value);

                await NotificationBusiness.SendEmailAsync(alert.Target, body);
            }
        }

        private int GetHourPeriod(Alert alert)
        {
            switch (alert.TypeOfPeriod)
            {
                case PeriodType.Hour:
                    return alert.PeriodValue;
                case PeriodType.Day:
                    return alert.PeriodValue*24;
                case PeriodType.Week:
                    return alert.PeriodValue*24*7;
                default:
                    return alert.PeriodValue;
            }
        }
    }
}