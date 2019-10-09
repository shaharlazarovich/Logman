using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Contracts
{
    public interface IAccountBusiness
    {
        Task<User> CreateUserAsync(User newUser);
        Task<User> GetUserAsync(long id);
        Task<User> GetUserAsync(string userName);
        Task<User> ActivateUserAsync(string activationKey);
        Task<bool> AuthenticateAsync(string username, string password);
        void Signout();
        User GetCurrentUser();
        void SendCreateUserConfirmationEmail(User user, string activationUrl);
        Task<string> ResetPasswordAsync(string userName);
        Task ChangePasswordAsync(long userId, string newPassword);
    }
}