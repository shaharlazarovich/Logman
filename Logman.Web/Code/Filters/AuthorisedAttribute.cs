using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Common.DomainObjects;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace Logman.Web.Code.Filters
{
    public class AuthorisedAttribute : AuthorizeAttribute
    {
        [Dependency]
        public IAccountBusiness AccountBusiness { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isLoggedIn =  AccountBusiness.GetCurrentUser() != null;
            if (!isLoggedIn)
            {
                isLoggedIn = CheckForCookie(httpContext);
            }
            return isLoggedIn;
        }

        private bool CheckForCookie(HttpContextBase httpContext)
        {
            var result = false;
            var cookie = httpContext.Request.Cookies[Constants.SpecialValues.AuthCookieName];
            if (cookie != null)
            {
                var userJson = cookie.Value;
                if (!string.IsNullOrEmpty(userJson))
                {
                    var user = JsonConvert.DeserializeObject<User>(userJson);
                    if (user != null && !string.IsNullOrEmpty(user.Username))
                    {
                        AccountBusiness.AuthenticateAsync(user.Username, user.Password).ContinueWith(
                            p =>
                            {
                                result = p.Result;
                            });
                    }
                }
            }
            return result;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"controller", "account"},
                {"action", "login"}
            });
        }
    }
}