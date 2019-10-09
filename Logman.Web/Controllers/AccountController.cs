using System;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Logman.Business;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Common.DomainObjects;
using Logman.Common.Logging;
using Logman.Web.Code.Filters;
using Logman.Web.Models.Account;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace Logman.Web.Controllers
{
    public class AccountController : Controller
    {
        [Dependency]
        public IAccountBusiness AccountBusiness { get; set; }

        [Dependency]
        public ILogger Logger { get; set; }

        [Dependency]
        public IEmailProvider EmailProvider { get; set; }

        public ActionResult CreateUser()
        {
            var model = new UserViewModel();
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user =
                        await
                            AccountBusiness.CreateUserAsync(new User
                            {
                                Username = model.Username,
                                Password = model.Password
                            });


                    if (user != null)
                    {
                        AccountBusiness.SendCreateUserConfirmationEmail(user,
                            Url.Action("ConfirmUser", "Account", new { key = user.ActivationKey }, Request.Url.Scheme));
                    }
                }
                catch (DuplicateNameException)
                {
                    ModelState.AddModelError("UserExists", "This email address is already registered.");
                    return View(model);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    ModelState.AddModelError("GeneralError", ex);
                    return View(model);
                }

                return RedirectToAction("ConfirmSignup");
            }
            return View(model);
        }

        public ActionResult ConfirmSignup(int mode = 0)
        {
            return View("PendingApproval");
        }


        public ActionResult SignOut()
        {
            AccountBusiness.Signout();
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> ConfirmUser(string key)
        {
            var activatedUser = await AccountBusiness.ActivateUserAsync(key);

            if (activatedUser != null && activatedUser.Enabled)
            {
                bool successfulLogin =
                    await AccountBusiness.AuthenticateAsync(activatedUser.Username, activatedUser.Password);
                if (successfulLogin)
                {
                    return RedirectToAction("Index", "Application");
                }
            }
            return new ContentResult { Content = "Not Confirmed." };
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var auth = await AccountBusiness.AuthenticateAsync(model.Username, model.Password);
                if (auth)
                {
                    if (model.RememberMe)
                    {
                        var currUser = AccountBusiness.GetCurrentUser();
                        if (currUser != null)
                        {
                            var authCookie = new HttpCookie(Constants.SpecialValues.AuthCookieName);
                            var serializedUser = JsonConvert.SerializeObject(currUser);
                            authCookie.Value = serializedUser;
                            Response.Cookies.Add(authCookie);
                        }
                    }

                    return RedirectToAction("Index", "Application");
                }
                else
                {
                    ModelState.AddModelError("customError", string.Format("Login failed for user {0}", model.Username));
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AccountBusiness.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("CustomError","The given email address does not exist.");
                    return View(model);
                }
                var newPassword = await AccountBusiness.ResetPasswordAsync(model.Email);

                // Send Email
                string emailTemplate = Server.MapPath("~/App_Data/Templates/ResetPassword.html");
                string templateBody = System.IO.File.ReadAllText(emailTemplate)
                    .Replace("@Name", model.Email)
                    .Replace("@Password", newPassword);
                await EmailProvider.SendAsync("", model.Email, "Your new Logman password", templateBody);
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        [Authorised]
        [System.Web.Mvc.HttpGet]
        public ActionResult ChangePassword()
        {
            var model =new ChangePasswordViewModel();
            var currentUser = AccountBusiness.GetCurrentUser();
            if (currentUser != null)
            {
                model.UserId = currentUser.Id;
            }
            return View(model);
        }

        [Authorised]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model != null && ModelState.IsValid &&  model.UserId.HasValue)
            {
                await AccountBusiness.ChangePasswordAsync(model.UserId.Value, model.NewPassword);
                return RedirectToAction("Login");
            }
            return View(model);
        }

    }
}