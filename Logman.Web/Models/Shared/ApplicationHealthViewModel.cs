using Logman.Common.DomainObjects;

namespace Logman.Web.Models.Shared
{
    public class ApplicationHealthViewModel
    {
        public string ApplicationName { get; set; }
        public long ApplicationId { get; set; }
        public int FatalCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int MaxFatalCount { get; set; }
        public int MaxErrorCount { get; set; }
        public int MaxWarningCount { get; set; }
        public string ContainerNamePrefix { get; set; }
        public ApplicationTrends AppTrends { get; set; }
    }
}