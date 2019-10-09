using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logman.Web.Code.Filters;

namespace Logman.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorised]
        public ActionResult Index()
        {
            return View();
        }
	}
}