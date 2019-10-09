namespace Logman.Common.DomainObjects
{
    public class ApplicationStatus
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int FatalCount { get; set; }
        public int DefaultRetainPeriodDays { get; set; }
        public int MaxFatalErrors { get; set; }
        public int MaxErrors { get; set; }
        public int MaxWarnings { get; set; }
    }
}