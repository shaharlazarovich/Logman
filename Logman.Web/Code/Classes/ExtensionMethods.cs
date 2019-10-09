using Logman.Common.DomainObjects;
using Logman.Web.Models.Account;

namespace Logman.Web.Code.Classes
{
    public static class ExtensionMethods
    {
        public static UserViewModel ToUserViewModel(this User user)
        {
            return new UserViewModel
            {
                Username = user.Username,
                Password = user.Password,
                RepeatPassword = user.Password
            };
        }
    }
}