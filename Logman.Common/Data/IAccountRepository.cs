using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Data
{
    public interface IAccountRepository
    {
        Task<User> CreateUserAsync(User newUser);
        Task<User> GetUserAsync(long id);
        Task<User> GetUserAsync(string userName);
        Task<User> ActivateUserAsync(string activationKey);
        Task ChangePasswordAsync(string userName, string password, string passwordSalt);
    }
}