using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quizmint.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["ProjectId"] = null;
            return View();
        }

        public ActionResult About()
        {
            Session["ProjectId"] = null;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Session["ProjectId"] = null;
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}