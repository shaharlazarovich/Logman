using System;

namespace Logman.Common.DomainObjects
{
    public class Alert
    {
        public int EventLevelValue { get; set; }
        public PeriodType TypeOfPeriod { get; set; }
        public int PeriodValue { get; set; }
        public int Value { get; set; }
        public NotificationType TypeOfNotification { get; set; }
        public string Target { get; set; }
        public long AppId { get; set; }
        public DateTime LastExecutionTime { get; set; }

        public long Id { get; set; }
    }

    public enum NotificationType
    {
        Email = 0,
        WebHook = 1
    }

    public enum PeriodType
    {
        Hour = 0,
        Day = 1,
        Week = 2
    }
}