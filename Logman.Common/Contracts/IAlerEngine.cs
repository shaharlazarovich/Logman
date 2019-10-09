using System.Collections.Generic;
using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Contracts
{
    public interface IAlerEngine
    {
        Task LoadAlertsAsync();
        Task ProcessAlertsAsync();
    }
}