using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logman.Common.Data
{
    public interface IRepository<T> where T : class
    {
        IUnitOfWork UnitOfWork { get; set; }
        Task<long> Add(T record);
        Task<T> GetByIdAsync(long id);
        Task<T> GetByIdAsync(string id);
        Task<List<T>> FindAllAsync();
    }
}