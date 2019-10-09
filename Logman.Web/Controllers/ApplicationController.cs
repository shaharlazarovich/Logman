using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using Logman.Business;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Common.DomainObjects;
using Logman.Common.Logging;
using Logman.Web.Code.Classes;
using Logman.Web.Code.Filters;
using Logman.Web.Models;
using Logman.Web.Models.Apps;
using Logman.Web.Models.Shared;
using Microsoft.Practices.Unity;
using Util = Logman.Business.Util;

namespace Logman.Web.Controllers
{
    public class ApplicationController : Controller
    {
        [Dependency]
        public IApplicationBusiness ApplicationBusiness { get; set; }

        [Dependency]
        public ILogger Logger { get; set; }

        [Dependency]
        public IEventBusiness EventBusiness { get; set; }

        [Dependency]
        public IAccountBusiness AccountBusiness { get; set; }

        /// <summary>
        ///     Displays the applications that are available to the current user.
        /// </summary>
        /// <returns></returns>
        [Authorised]
        public async Task<ActionResult> Index()
        {
            try
            {
                List<Application> allApps = await ApplicationBusiness.FindAllAsync();
                User user = AccountBusiness.GetCurrentUser();
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }


                IEnumerable<long> appIds = (await ApplicationBusiness.GetAppsOfUserAsync(user.Id)).Select(p => p.Id);

                Util.Clear<GaugeData>(Code.Classes.Util.GetLayoutViewModel().Gauges);
                Code.Classes.Util.GetLayoutViewModel().Lines.Clear();
                var viewModel = new ApplicationViewModel();
                foreach (int appId in appIds)
                {
                    Application app = await ApplicationBusiness.GetByIdAsync(appId);
                    if (app.Id > 0)
                    {
                        ApplicationStatus appStatus = await ApplicationBusiness.GetAppStatusAsync(app.Id);
                        ApplicationTrends appTrend =  await ApplicationBusiness.GetApplicationTrendAsync(app.Id);

                        app.Status = appStatus;
                        app.Trend = appTrend;
                        viewModel.Applications.Add(app);
                    }
                }
                return View("Index", viewModel);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
                throw;
            }
        }

        [Authorised]
        public async Task<ActionResult> AppEvents(long appId, int pageNumber = 0, string keywords = "",
            DateTime? fromDate = null, EventLevel? eventLevel = null,
            bool returnPartial = false)
        {
            try
            {
                fromDate = !fromDate.HasValue
                    ? DateTime.UtcNow.AddDays(-1*Util.GetRetentionDays())
                    : fromDate.Value.ToUniversalTime();

                if (!eventLevel.HasValue)
                {
                    eventLevel = EventLevel.Fatal | EventLevel.Error | EventLevel.Warning | EventLevel.Information;
                }


                ApplicationEvents events =
                    await
                        ApplicationBusiness.GetApplicationEventsAsync(appId, pageNumber,
                            Constants.SpecialValues.PageSize, keywords, fromDate, DateTime.UtcNow, eventLevel);

                if (returnPartial)
                {
                    return PartialView("EventGrid", events.Events);
                }

                int pNumber = pageNumber + 1;

                NameValueCollection nameValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                nameValues.Set("pageNumber", pNumber.ToString(CultureInfo.InvariantCulture));
                nameValues.Set("returnPartial", "true");
                nameValues.Set("fromDate", fromDate.Value.ToLocalTime().ToString(CultureInfo.InvariantCulture));
                nameValues.Set("eventLevel", ((int) eventLevel.Value).ToString(CultureInfo.InvariantCulture));
                nameValues.Set("keywords", keywords);

                string url = Request.Url.AbsolutePath;
                events.NewQueryString = string.Format("{0}?{1}", url, nameValues);
                return View(events);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new ContentResult {Content = "An error occured."};
            }
        }

        public async Task<ActionResult> GetEvent(long id, string appKey)
        {
            try
            {
                Event eventEntry = await EventBusiness.GetByIdAsync(id, appKey);
                return PartialView("EventDetails", eventEntry);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return new ContentResult {Content = "An error occured."};
            }
        }

        /// <summary>
        ///     Invites users to join an app.
        /// </summary>
        /// <param name="id">The unique databae Id of the application.</param>
        /// <returns></returns>
        [Authorised]
        public async Task<ActionResult> InviteUser(int id)
        {
            User user = AccountBusiness.GetCurrentUser();
            if (user != null)
            {
                Application app = await ApplicationBusiness.GetByIdAsync(id);
                if (app != null)
                {
                    if (app.Users.ContainsKey(user.Id) &&
                        (app.Users[user.Id] == Roles.Owner || app.Users[user.Id] == Roles.Admin))
                    {
                        var model = new InviteUserViewModel();
                        return View(model);
                    }
                    return new HttpUnauthorizedResult("You do not have access to invite users for this application.");
                }
                return HttpNotFound("The application was not found.");
            }
            return RedirectToAction("Index", "Application");
        }

        [Authorised]
        [HttpPost]
        public async Task<ActionResult> InviteUser(InviteUserViewModel model)
        {
            User user = await AccountBusiness.GetUserAsync(model.UserEmail);
            if (user == null)
            {
                var bytes = new byte[8];
                new RNGCryptoServiceProvider().GetNonZeroBytes(bytes);
                user =
                    await
                        AccountBusiness.CreateUserAsync(new User
                        {
                            ActivationKey = Guid.NewGuid().ToString(),
                            Enabled = false,
                            Password = string.Join("", bytes.Select(c => c.ToString("x2")))
                        });
            }
            //ToDo: Add user to App User table
            //ToDo: Email user about this
            return new EmptyResult(); //ToDo: Fix this
        }

        [HttpGet]
        [Authorised]
        public ActionResult CreateApplication()
        {
            var model = new AppCreateViewModel
            {
                IsEditMode = false,
                Enabled = true,
                DefaultRetainPeriodDays = Util.GetRetentionDays(),
                MaxErrors = 100,
                MaxFatalErrors = 100,
                MaxWarnings = 100
            };
            return View(model);
        }

        [HttpPost]
        [Authorised]
        public async Task<ActionResult> CreateApplication(AppCreateViewModel model)
        {
            var createAppModel = new Application
            {
                AppName = model.AppName,
                DefaultRetainPeriodDays = model.DefaultRetainPeriodDays,
                Enabled = model.Enabled,
                MaxErrors = model.MaxErrors,
                MaxFatalErrors = model.MaxFatalErrors,
                MaxWarnings = model.MaxWarnings
            };

            try
            {
                validateCreateAppViewModel(model);
                Application newApp = await ApplicationBusiness.CreateApplicationAsync(createAppModel);
                if (newApp == null)
                {
                    throw new ApplicationException("Could not create the application!");
                }
                return RedirectToAction("Index", "Application");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("CustomError", ex);
                return View(model);
            }
        }

        [HttpPost]
        [Authorised]
        public async Task<ActionResult> UpdateApplication(AppCreateViewModel model)
        {
            var createAppModel = new Application
            {
                AppName = model.AppName,
                AppKey = model.AppKey,
                Id = model.Id,
                DefaultRetainPeriodDays = model.DefaultRetainPeriodDays,
                Enabled = model.Enabled,
                MaxErrors = model.MaxErrors,
                MaxFatalErrors = model.MaxFatalErrors,
                MaxWarnings = model.MaxWarnings
            };

            try
            {
                validateCreateAppViewModel(model);
                Application newApp = await ApplicationBusiness.UpdateApplicationAsync(createAppModel);
                if (newApp == null)
                {
                    throw new ApplicationException("Could not create the application!");
                }
                return RedirectToAction("Index", "Application");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("CustomError", ex);
                return View("CreateApplication", model);
            }
        }


        [HttpGet]
        [Authorised]
        public async Task<ActionResult> UpdateApplication(long Id)
        {
            Application app = await ApplicationBusiness.GetByIdAsync(Id);
            if (app != null && !string.IsNullOrEmpty(app.AppKey))
            {
                var model = new AppCreateViewModel
                {
                    IsEditMode = true,
                    Enabled = app.Enabled,
                    DefaultRetainPeriodDays = app.DefaultRetainPeriodDays,
                    MaxErrors = app.MaxErrors,
                    MaxFatalErrors = app.MaxFatalErrors,
                    MaxWarnings = app.MaxWarnings,
                    AppName = app.AppName,
                    AppKey = app.AppKey,
                    Id = Id
                };
                return View("CreateApplication", model);
            }

            return new HttpNotFoundResult("The application that you requests could not be found.");
        }


        private void validateCreateAppViewModel(AppCreateViewModel model)
        {
            if (model.DefaultRetainPeriodDays == 0)
            {
                throw new ArgumentException("Retention period must be between 1 and 30 days");
            }

            if (model.MaxFatalErrors == 0)
            {
                throw new ArgumentException("Max. number of fatal errors must be bigger than 1. Suggested value is 100");
            }

            if (model.MaxErrors == 0)
            {
                throw new ArgumentException("Max. number of errors must be bigger than 1. Suggested value is 100");
            }

            if (model.MaxWarnings == 0)
            {
                throw new ArgumentException("Max. number of warnings must be bigger than 1. Suggested value is 100");
            }
        }

        [HttpGet]
        public ActionResult AddAlert(int id)
        {
            var model = new AlertViewModel
            {
                AppId = id,
                IncludesFatals = true,
                TypeOfNotification = (short) NotificationType.Email,
                TypeOfPeriod = (short) PeriodType.Hour,
                PeriodValue = Constants.SpecialValues.AlertDefaultPeriodValue
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddAlert(AlertViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    if (!model.IncludesErrors && !model.IncludesFatals && !model.IncludesWarnings)
                    {
                        ModelState.AddModelError("Custom", "At least one type of event must be selected.");
                        return View(model);
                    }

                    int eventLevelValue = 0;
                    if (model.IncludesFatals)
                    {
                        eventLevelValue = (int) EventLevel.Fatal;
                    }

                    if (model.IncludesErrors)
                    {
                        eventLevelValue = eventLevelValue | (int) EventLevel.Error;
                    }

                    if (model.IncludesWarnings)
                    {
                        eventLevelValue = eventLevelValue | (int) EventLevel.Warning;
                    }


                    await EventBusiness.AddAlertAsync(new Alert
                    {
                        EventLevelValue = eventLevelValue,
                        PeriodValue = model.PeriodValue,
                        Target = model.Target,
                        TypeOfPeriod = (PeriodType) model.TypeOfPeriod,
                        TypeOfNotification = (NotificationType) model.TypeOfNotification,
                        Value = model.Value,
                        AppId = model.AppId
                    });
                    return RedirectToAction("ViewAlerts", new {appId = model.AppId});
                }
                return View(model);
            }

            ModelState.AddModelError("Custom", "Invalid model was passed.");
            return View(new AlertViewModel());
        }

        public async Task<ActionResult> ViewAlerts(long appId)
        {
            List<Alert> alerts = await ApplicationBusiness.GetAlertsOfApp(appId);
            ViewBag.AppId = appId;
            return View(alerts);
        }

        public async Task<ActionResult> RemoveAlert(long alertId)
        {
            //ToDo: Remvoe alert;
            return Json(new {name = "aref"});
        }

        public async Task<ActionResult> ManageUsers(long appId)
        {
            var app = await ApplicationBusiness.GetByIdAsync(appId);
            var model = new AppUsersViewModel() {AppId = app.Id, AppName = app.AppName};
            var appUsers = await ApplicationBusiness.GetApplicationUsersAsync(appId);
            model.AppUsers.AddRange(appUsers);
            return View(app);
        }
    }
}