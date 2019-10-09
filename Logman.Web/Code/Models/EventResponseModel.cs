using System;
using System.Runtime.Serialization;
using Logman.Common.DomainObjects;

namespace Logman.Web.Code.Models
{
    [DataContract(IsReference = false, Namespace = "")]
    public class EventResponseModel
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
        public EventResponseModel InnerEvent { get; set; }

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
    }
}