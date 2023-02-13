using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DigiProc;
using DigiProc.Helpers;
using System.Diagnostics;

namespace DigiProc.Views.User
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authenticate()
        {
            return View();
        }
        public ActionResult IDChallenge()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string usrname, string pwd)
        {
            try
            {
                usrname = usrname.Replace(@"panafricansl.com", string.Empty);
                ConfigurationHelper Cfg = new ConfigurationHelper();
                var objUser = Cfg.GetUser(usrname, Security.Hashing.CreateMD5Hash(pwd));

                if (objUser.isActive == @"Yes")
                {
                    var usModules = new Utility() { }.getUserModules(objUser.username); //obj.getUserModules(session.userName);

                    var _session = new UserSession()
                    {
                        userDepartment = new Department { Name = objUser.nameOfDepartment },
                        userName = objUser.username,
                        userProfile = objUser.PrManager.nameOfProfile,
                        moduleString = objUser.PrManager.contentOfProfile,
                        modules = usModules,
                        approverTag = objUser.userTag,
                        bioName = string.Format("{0} {1} {2}",objUser.sname,objUser.fname,objUser.onames)
                    };

                    Session["userSession"] = _session;

                    return Json(new { status = true, data = objUser }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("Main", "Home");
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAssignedModules()
        {
            //system uses the username session variable to get the modules assigned to this user
            try
            {
                UserSession session = (UserSession)Session["userSession"];

                return Json(session.modules, JsonRequestBehavior.AllowGet);
            }
            catch (Exception errmsg)
            {
                return Json(new { status = false, error = $"error: {errmsg.Message}" });
            }
        }

    }
}