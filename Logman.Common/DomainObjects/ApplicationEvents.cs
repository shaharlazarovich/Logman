using System.Collections.Generic;

namespace Logman.Common.DomainObjects
{
    public class ApplicationEvents
    {
        private readonly List<Event> _events;
        private readonly Dictionary<string, int> _topEvents;

        public ApplicationEvents()
        {
            _events = new List<Event>();
            _topEvents = new Dictionary<string, int>();
        }

        public List<Event> Events
        {
            get { return _events; }
        }

        public Dictionary<string, int> TopEvents
        {
            get { return _topEvents; }
        }

        public string NewQueryString { get; set; }
    }
}