using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Data
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<Event> GetChildAsync(long id);
        Task<int> AddAlert(Alert alert);
    }
}