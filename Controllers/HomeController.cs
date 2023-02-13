using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DigiProc.Helpers;

namespace DigiProc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Desktop()
        {
            ViewBag.Message = @"eProcurement System v1.0";

            return View("Desktop");
        }

        public ActionResult IDChallenge()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return Json(new { status = true, data = $"logging out..." },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Main()
        {
            return View();
        }
        

    }
}