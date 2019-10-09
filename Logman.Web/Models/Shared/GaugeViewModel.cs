namespace Logman.Web.Models.Shared
{
    public class GaugeViewModel
    {
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public string ContainerName { get; set; }
        public string Label { get; set; }
        public long AppId { get; set; }
    }
}