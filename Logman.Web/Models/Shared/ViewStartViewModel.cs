using System.Collections.Concurrent;

namespace Logman.Web.Models.Shared
{
    public class ViewStartViewModel
    {
        private readonly ConcurrentBag<GaugeData> _gaugeData;
        private readonly ConcurrentBag<LineData> _lineData;

        public ViewStartViewModel()
        {
            _gaugeData = new ConcurrentBag<GaugeData>();
            _lineData = new ConcurrentBag<LineData>();
        }

        public ConcurrentBag<GaugeData> Gauges
        {
            get { return _gaugeData; }
        }

        public ConcurrentBag<LineData> Lines
        {
            get { return _lineData; }
        }
    }
}