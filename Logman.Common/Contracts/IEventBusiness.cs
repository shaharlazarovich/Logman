using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Contracts
{
    public interface IEventBusiness
    {
        Task<long> RegisterEventAsync(Event log);
        Task<Event> GetByIdAsync(long id, string appKey);
        Task<Event> GetChildAsync(long id);
        Task<int> AddAlertAsync(Alert alert);
    }
}