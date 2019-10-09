using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Common.Data;
using Logman.Common.DomainObjects;
using Microsoft.Practices.Unity;

namespace Logman.Business.Account
{
    public class AccountBusiness : IAccountBusiness
    {
        [Dependency]
        public IDataAccessLayer DataAccessLayer { get; set; }

        [Dependency]
        public IEmailProvider EmailProvider { get; set; }

        [Dependency]
        public ICacheProvider CacheProvider { get; set; }

        [Dependency]
        public ISessionProvider SessionProvider { get; set; }

        public async Task<User> CreateUserAsync(User newUser)
        {
            string plainPassword = newUser.Password;
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repos = DataAccessLayer.GetAccountRepository(uow);

                User theUser = await repos.GetUserAsync(newUser.Username);
                if (theUser == null)
                {
                    newUser.PasswordSalt = CryptoHelper.GenerateSalt();
                    newUser.Password = CryptoHelper.Hash(newUser.Password + newUser.PasswordSalt);
                    User user = await repos.CreateUserAsync(newUser);
                    user.Password = plainPassword;
                    return user;
                }
                throw new DuplicateNameException();
            }
        }

        public async Task<User> GetUserAsync(long id)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repos = DataAccessLayer.GetAccountRepository(uow);
                return await repos.GetUserAsync(id);
            }
        }

        public async Task<User> GetUserAsync(string userName)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repos = DataAccessLayer.GetAccountRepository(uow);
                return await repos.GetUserAsync(userName);
            }
        }

        public async Task<User> ActivateUserAsync(string activationKey)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repos = DataAccessLayer.GetAccountRepository(uow);
                return await repos.ActivateUserAsync(activationKey);
            }
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var currentSessionKey = SessionProvider.GetTyped<User>(Constants.SpecialValues.CurrentUserSessionKey);
            if (currentSessionKey != null)
            {
                return true;
            }

            // User is not logged in. Check the credentials.
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repos = DataAccessLayer.GetAccountRepository(uow);
                User user = await repos.GetUserAsync(username);
                if (user != null)
                {
                    string saltedPassword = password + user.PasswordSalt;
                    string hashedPassword = CryptoHelper.Hash(saltedPassword);
                    if (hashedPassword == user.Password && user.Enabled)
                    {
                        SessionProvider[Constants.SpecialValues.CurrentUserSessionKey] = user;
                        return true;
                    }
                }
            }
            return false;
        }


        public void Signout()
        {
            var currentUser = SessionProvider.GetTyped<User>(Constants.SpecialValues.CurrentUserSessionKey);
            if (currentUser != null)
            {
                SessionProvider.Remove(Constants.SpecialValues.CurrentUserSessionKey);
            }
        }

        public User GetCurrentUser()
        {
            return SessionProvider.GetTyped<User>(Constants.SpecialValues.CurrentUserSessionKey);
        }

        public void SendCreateUserConfirmationEmail(User user, string activationUrl)
        {
            string templateBody =
                "Dear @Name,<br>Your LogMan account is pending confirmation.Please clcick here to activate your account: <a href='@Link'>Activate</a><br>";
            var imagePaths = new Dictionary<string, string>();
            if (HttpContext.Current != null)
            {
                string emailTemplate = HttpContext.Current.Server.MapPath("~/App_Data/Templates/ConfirmEmail.html");
                templateBody = File.ReadAllText(emailTemplate)
                    .Replace("@Name", user.Username)
                    .Replace("@Link", activationUrl)
                    .Replace("@Password", user.Password);
                imagePaths.Add("myImageID", HttpContext.Current.Server.MapPath("~/Content/Images/LogManLogo.jpg"));
            }

            EmailProvider.SendAsync("", user.Username, "Confirm Your LogMan Account!", templateBody, imagePaths);
        }

        public async Task<string> ResetPasswordAsync(string username)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repository = DataAccessLayer.GetAccountRepository(uow);
                string passwordSalt = CryptoHelper.GenerateSalt();
                string newPassword = Guid.NewGuid().ToString().Replace("-", "");
                string saltedPassword = CryptoHelper.Hash(string.Format("{0}{1}", newPassword, passwordSalt));
                await repository.ChangePasswordAsync(username, saltedPassword, passwordSalt);
                return newPassword;

            }
        }

        public async Task ChangePasswordAsync(long userId, string newPassword)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IAccountRepository repository = DataAccessLayer.GetAccountRepository(uow);
                User user = await repository.GetUserAsync(userId);
                if (user != null)
                {
                    string passwordSalt = CryptoHelper.GenerateSalt();
                    string saltedPassword = CryptoHelper.Hash(string.Format("{0}{1}", newPassword, passwordSalt));
                    await repository.ChangePasswordAsync(user.Username, saltedPassword, passwordSalt);
                }
            }
        }

        public string GetSessionKey(string userName)
        {
            string userKey = string.Format("{0}_{1}", Constants.SpecialValues.SessionPrefix, userName);
            return CacheProvider.Get<string>(userKey);
        }
    }
}