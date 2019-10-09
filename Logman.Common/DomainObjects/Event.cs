using System;
using System.Runtime.Serialization;

namespace Logman.Common.DomainObjects
{
    [DataContract(Namespace = "")]
    public class Event
    {
        [IgnoreDataMember]
        public long Id { get; set; }

        [DataMember]
        public string ProviderName { get; set; }

        [DataMember]
        public EventLevel EventLevel { get; set; }

        [DataMember]
        public string Keywords { get; set; }

        [DataMember]
        public DateTime TimeCreated { get; set; }

        [DataMember]
        public long? ParentId { get; set; }

        [DataMember]
        public string ComputerName { get; set; }

        [DataMember]
        public string IpAddress { get; set; }

        [DataMember]
        public string UserAgent { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ExtendedInformation { get; set; }

        [IgnoreDataMember]
        public long ApplicationId { get; set; }

        [DataMember]
        public Event InnerEvent { get; set; }
    }
}