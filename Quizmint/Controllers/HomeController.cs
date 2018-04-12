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
            ClearSessionProject();
            return View();
        }

        public ActionResult About()
        {
            ClearSessionProject();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ClearSessionProject();
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [NonAction]
        public void ClearSessionProject()
        {
            Session["ProjectId"] = null;
            Session["ProjectName"] = null;
        }
    }
}