using System.Collections.Concurrent;
using System.Net;
using System.Web;
using Logman.Common.DomainObjects;

namespace Logman.Business
{
    public static class Util
    {
        public static string GetClientIp(HttpRequest request)
        {
            string visitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                visitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.Current.Request.UserHostAddress))
            {
                visitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return visitorsIPAddr;
        }

        public static string GetClientComputerName(string clientIP)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(clientIP);
                return hostEntry.HostName;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static void PopulateClientInfo(Event record, HttpRequest request)
        {
            var ip = GetClientIp(request);
            var machineName = GetClientComputerName(ip);

            record.IpAddress = ip;
            record.ComputerName = machineName;
            record.UserAgent = request.UserAgent;
        }

        public static void Clear<T>(this ConcurrentBag<T> instance)
        {
            while (!instance.IsEmpty)
            {
                T someItem;
                instance.TryTake(out someItem);
            }
        }

        public static string GetEventCacheKey(long eventId)
        {
            return string.Format("event_{0}", eventId);
        }


        public static  int GetLogLifespanHours()
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["LogLifespanHours"]);
        }

        public static int GetRetentionDays()
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["retentionDays"]);
        }

        public static int SessionLifeTimeMinutes()
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["sessionLifeTimeMinutes"]);
        }
    }
}