using System.Collections.Concurrent;

namespace Logman.Common.DomainObjects
{
    public class Application
    {
        private readonly ConcurrentDictionary<long, Roles> _users;

        public Application()
        {
            _users = new ConcurrentDictionary<long, Roles>();
        }

        public long Id { get; set; }
        public string AppName { get; set; }
        public string AppKey { get; set; }
        public bool Enabled { get; set; }
        public int DefaultRetainPeriodDays { get; set; }
        public int MaxFatalErrors { get; set; }
        public int MaxErrors { get; set; }
        public int MaxWarnings { get; set; }

        public ApplicationStatus Status { get; set; }
        public ApplicationTrends Trend { get; set; }

        public ConcurrentDictionary<long, Roles> Users
        {
            get { return _users; }
        }
    }
}