using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Logman.Business;
using Util = Logman.Web.Code.Classes.Util;

namespace Logman.Web.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            Util.GetLayoutViewModel().Gauges.Clear();
            Util.GetLayoutViewModel().Lines.Clear();

            return View();
        }
    }
}
