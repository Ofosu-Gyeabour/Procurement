using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DigiProc;
using DigiProc.Helpers;
using System.Diagnostics;
using System.Collections;

namespace DigiProc.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult IDChallenge()
        {
            return View();
        }

        #region User-Getters

        [HttpGet]
        public JsonResult GetUserProfiles()
        {
            //1= active profiles, 0=inactive profiles
            try
            {
                var cfg = new ConfigurationHelper() { };
                var active_profiles = cfg.GetProfiles();
                return Json(new { status = true, data = active_profiles },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveUserProfile(string _profile, string _profContent, int _profStatus)
        {
            try
            {
                var Cfg = new ConfigurationHelper();

                var appProfile = new Profile() { 
                    profileName = _profile,
                    profileContent = _profContent.TrimEnd(','),
                    inUse = _profStatus
                };

                bool bln = Cfg.CreateUserProfile(appProfile);

                return Json(new { status = bln, data = appProfile },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCurrentProfileOfUser(string u)
        {
            try
            {
                var Cfg = new ConfigurationHelper();
                var obj = Cfg.GetUser(u);

                return Json(new { status = true, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetApplicationModulesForProfile(string _profile)
        {
            //get the modules associated with a profile
            try
            {
                List<Module> module_data = null;

                var Cfg = new ConfigurationHelper();
                var profileObj = Cfg.GetProfile(_profile);

                if (profileObj.contentOfProfile.Length > 0)
                {
                    module_data = new List<Module>();

                    string[] str = profileObj.contentOfProfile.TrimEnd(',').Split(',');
                    if (str.Length > 0)
                    {
                        foreach(var s in str)
                        {
                            var moduleObj = Cfg.GetModule(s);
                            module_data.Add(moduleObj);
                        }
                    }
                }

                return Json(new { status = true, data = module_data },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AmendUserProfile(string u, string pro)
        {
            try
            {
                /*
                    change profile name in the usr table
                    delete current profiles in the usrmodule table
                    create new profiles in the usrmodule table
                */
                int success = 0; int failed = 0;
                int moduleCount = 0;

                var Cfg = new ConfigurationHelper();
                var usrMgr = Cfg.GetUser(u);
                var objUser = new Usr() { Id = usrMgr.Id, usrname = usrMgr.username, uProfile = pro };

                bool bln = Cfg.AmendUserProfile(objUser);
                if (bln)
                {
                    if (Cfg.DeleteUserModule(objUser))
                    {
                        var oProfile = Cfg.GetProfile(pro);
                        if (oProfile.contentOfProfile.Length > 0)
                        {
                            string[] str = oProfile.contentOfProfile.TrimEnd(',').Split(',');
                            moduleCount = str.Length;
                            if (moduleCount > 0)
                            {
                                foreach (var s in str)
                                {
                                    var mod = Cfg.GetModule(s);

                                    //creating user module object
                                    var um = new UserModule() {
                                        UserName = u,
                                        ModuleID = mod.ModuleID,
                                        DateAssigned = DateTime.Now
                                    };

                                    if (Cfg.CreateUserModule(um) > 0) { success += 1; } else { failed += 1; }
                                }
                            }
                        }
                    }

                    return Json(new { status = bln, data = $"Modules in {pro} = {moduleCount.ToString()}. Modules created success ={success.ToString()}" },JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { status = false, error = $"user profile could not be created\r\nPlease contact Administrator" },JsonRequestBehavior.AllowGet); }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, error = $"{ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            //gets users in the data store
            try
            {
                var helper = new ConfigurationHelper() { };
                var user_list = helper.GetUsers();

                return Json(new { status = true, data = user_list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetApplicationModules()
        {
            //gets the modules that makes up the application
            try
            {
                var application_modules = new ConfigurationHelper { }.GetModules();
                return Json(new { status = true, data = application_modules},JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = true, error = $"error: {ex.Message}" });
            }
        }

        #endregion

        public JsonResult saveUserCredentials(string usr, string pwd, string stat, string isAd, string prof, int dId, string tg, string sname, string fname, string onames)
        {
            try
            {
                int success = 0; int failed = 0;

                var cfg = new ConfigurationHelper() { };

                var objUser = new Usr() {
                    usrname = usr,
                    usrpassword = Security.Hashing.CreateMD5Hash(pwd),
                    deptId = dId,
                    isActive = stat == @"ACTIVE" ? 1:0,
                    isLogged = 0,
                    isAD = isAd == @"YES"? 1: 0,
                    isAdmin = 0,
                    uProfile = prof,
                    tag = tg,
                    surname = sname,
                    firstname = fname,
                    othernames = onames
                };

                int recordId = cfg.CreateUserAccount(objUser);
                if (recordId > 0)
                {
                    //create user modules
                    var objProfile = cfg.GetProfile(objUser.uProfile);
                    if (objProfile.contentOfProfile != string.Empty) {
                        string[] str = objProfile.contentOfProfile.Split(',');

                        //iteration
                        foreach(var s in str)
                        {
                            //get the module id
                            var moduleObj = cfg.GetModule(s);

                            var objUserModule = new UserModule() 
                            { 
                                UserName = usr,
                                ModuleID = moduleObj.ModuleID,
                                DateAssigned = DateTime.Now
                            };

                            if (cfg.CreateUserModule(objUserModule) > 0) { success += 1; } else { failed += 1; }
                        }
                    }
                }

                return Json(new { status = true, data = $"User account {usr} created successfully with {success.ToString()} modules added" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Login(string usrname, string pwd)
        {
            try
            {
                ConfigurationHelper Cfg = new ConfigurationHelper();
                var objUser = Cfg.GetUser(usrname, Security.Hashing.CreateHash(pwd));

                if (objUser.isActive == @"Yes")
                {
                    var _session = new UserSession()
                    {
                        userDepartment = new Department { Name = objUser.nameOfDepartment },
                        userName = objUser.username,
                        userProfile = objUser.PrManager.nameOfProfile,
                        moduleString = objUser.PrManager.contentOfProfile
                    };

                    Session["userSession"] = _session;

                    return Json(new { status = true, data = objUser },JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false },JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

    }
}