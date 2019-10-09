using System.Collections.Generic;
using Logman.Common.DomainObjects;

namespace Logman.Web.Models.Apps
{
    public class ApplicationViewModel
    {
        private readonly List<Application> _applications;

        public ApplicationViewModel()
        {
            _applications = new List<Application>();
        }

        public List<Application> Applications
        {
            get { return _applications; }
        }
    }
}