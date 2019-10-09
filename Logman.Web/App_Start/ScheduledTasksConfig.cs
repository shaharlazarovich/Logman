using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Logman.Common.Contracts;
using Microsoft.Practices.Unity;

namespace Logman.Web.App_Start
{
    public static class ScheduledTasksConfig
    {
        private const long AlertPullInterval = 1000*60*60;
            // one hour : because the minimum interval for alerts is one hour

        private static Timer _alertTimer;
        private static readonly IAlerEngine AlertEngine = UnityConfig.GetConfiguredContainer().Resolve<IAlerEngine>();
        private static bool _alertsBeingProcessed;
        private static readonly object LockObject = new object();

        private static void AlertTimerElapsed(object state)
        {
            lock (LockObject)
            {
                if (_alertsBeingProcessed) return;
                _alertsBeingProcessed = true;
            }

            Task task1 = AlertEngine.LoadAlertsAsync();
            task1.Wait();

            Task task2 = AlertEngine.ProcessAlertsAsync();
            task2.Wait();

            lock (LockObject)
            {
                _alertsBeingProcessed = false;
            }

            _alertTimer.Change(AlertPullInterval, Timeout.Infinite);
        }


        public static void ConfigureAlerts()
        {
            _alertTimer = new Timer(AlertTimerElapsed, null, 0, Timeout.Infinite);
        }
    }
}