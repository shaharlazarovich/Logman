using System;
using System.Collections.Concurrent;

namespace Logman.Common.DomainObjects
{
    public class ApplicationTrends
    {
        private readonly ConcurrentDictionary<DateTime, int> _error;
        private readonly ConcurrentDictionary<DateTime, int> _fatal;
        private readonly ConcurrentDictionary<DateTime, int> _warning;

        public ApplicationTrends()
        {
            _fatal = new ConcurrentDictionary<DateTime, int>();
            _error = new ConcurrentDictionary<DateTime, int>();
            _warning = new ConcurrentDictionary<DateTime, int>();
        }

        public ConcurrentDictionary<DateTime, int> Fatals
        {
            get { return _fatal; }
        }

        public ConcurrentDictionary<DateTime, int> Errors
        {
            get { return _error; }
        }

        public ConcurrentDictionary<DateTime, int> Warnings
        {
            get { return _warning; }
        }
    }
}