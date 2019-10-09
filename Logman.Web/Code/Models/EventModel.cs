using Logman.Common.DomainObjects;

namespace Logman.Web.Code.Models
{
    public class EventModel
    {
        public string AppKey { get; set; }
        public string ProviderName { get; set; }
        public EventLevel EventLevel { get; set; }
        public string Keywords { get; set; }
        public EventModel InnerEvent { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string ExtendedInformation { get; set; }
    }
}