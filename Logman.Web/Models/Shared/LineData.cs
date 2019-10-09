using System.Collections.Generic;

namespace Logman.Web.Models.Shared
{
    public class LineData
    {
        private readonly Dictionary<string, int> _data;

        public LineData()
        {
            _data = new Dictionary<string, int>();
        }

        public string XAxisName { get; set; }
        public string YAxisName { get; set; }
        public string ChartTitle { get; set; }
        public string ContainerName { get; set; }

        public Dictionary<string, int> Data
        {
            get { return _data; }
        }
    }
}