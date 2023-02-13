using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using DigiProc;

using System.Diagnostics;

namespace DigiProc.Helpers
{
    public interface IConfigurationHelper
    {
        int CreateUserAccount(Usr item);
        bool AmendUserProfile(Usr item);

        bool CreateUserProfile(Profile item);

        int CreateUserModule(UserModule item);

        
    }
    public class ConfigurationHelper
        :IConfigurationHelper
    {

        #region Private properties

        private ProcurementDbEntities config = new ProcurementDbEntities();

        #endregion

        public int CreateUserAccount(Usr item)
        {
            //creates user account
            int _id = 0;

            try
            {
                config.Usrs.Add(item);
                config.SaveChanges();
                return item.Id;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return _id;
            }
        }

        public int CreateUserModule(UserModule item)
        {
            //create a user module
            int _id = 0;

            try
            {
                config.UserModules.Add(item);
                config.SaveChanges();

                return item.UserModuleID;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return _id;
            }
        }

        public bool DeleteUserModule(Usr item)
        {
            //deletes all user modules in the data store
            try
            {
                var module_list = config.UserModules.Where(u => u.UserName == item.usrname).ToList();
                if (module_list.Count() > 0)
                {
                    foreach(var m in module_list)
                    {
                        config.UserModules.Remove(m);
                        config.SaveChanges();
                    }

                    return true;
                }
                else { return false; }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool CreateUserProfile(Profile item)
        {
            try
            {
                config.Profiles.Add(item);
                config.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool AmendUserProfile(Usr item)
        {
            try
            {
                var obj = config.Usrs.Where(u => u.Id == item.Id).FirstOrDefault();
                if (obj != null)
                {
                    obj.uProfile = item.uProfile;
                    config.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch(Exception exc)
            {
                Debug.Print(exc.Message);
                return false;
            }
        }

        #region Getters

        public List<ProfileManager> GetProfiles()
        {
            //gets the list of profiles in the data store using the status (1=in use, 0=inactive)

            List<ProfileManager> profileManagers = new List<ProfileManager>();

            try
            {
                var dta = config.Profiles.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var obj = new ProfileManager() { 
                            Id = d.Id,
                            nameOfProfile = d.profileName,
                            contentOfProfile = d.profileContent
                        };

                        if (d.inUse == 1) { obj.profileInUse = @"Yes"; } else { obj.profileInUse = @"No"; }

                        profileManagers.Add(obj);
                    }
                }

                return profileManagers;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return profileManagers;
            }
        }

        public ProfileManager GetProfile(string strProfile)
        {
            //gets profile using name of profile
            ProfileManager profileManager = new ProfileManager();

            try
            {
                var obj = config.Profiles.Where(p => p.profileName == strProfile).FirstOrDefault();
                if (obj != null)
                {
                    profileManager.Id = obj.Id;
                    profileManager.nameOfProfile = obj.profileName;
                    profileManager.contentOfProfile = obj.profileContent;
                    profileManager.profileInUse = obj.inUse == 1 ? @"Yes" : @"No";

                    return profileManager;
                }
                else { return profileManager; }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return profileManager;
            }
        }
        
        public Module GetModule(string strModule)
        {
            Module module = new Module();

            try
            {
                var obj = config.Modules.Where(m => m.SystemName == strModule).FirstOrDefault();
                if (obj != null)
                {
                    module.ModuleID = obj.ModuleID;
                    module.SystemName = obj.SystemName;
                    module.PublicName = obj.PublicName;
                    module.DateAssigned = obj.DateAssigned;
                }

                return module;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return module;
            }
        }

        public List<Module> GetModules()
        {
            List<Module> app_modules = new List<Module>();

            try
            {
                var dta = config.Modules.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var m = new Module() {
                            ModuleID = d.ModuleID,
                            SystemName = d.SystemName,
                            PublicName = d.PublicName,
                            DateAssigned = d.DateAssigned
                        };

                        app_modules.Add(m);
                    }
                }

                return app_modules;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return app_modules;
            }
        }

        public List<UserManager> GetUsers()
        {
            //gets list of managers in the data store
            List<UserManager> userManagers = new List<UserManager>();

            try
            {
                var _data = config.Usrs.ToList();
                if (_data.Count() > 0)
                {
                    foreach(var d in _data)
                    {
                        var obj = new UserManager() { 
                            Id = d.Id,
                            username = d.usrname,
                            isActive = d.isActive == 1? @"Yes": @"No",
                            isLogged = d.isLogged == 1? @"Yes": @"No",
                            nameOfDepartment = new Utility() { }.getDepartment(d.deptId).Name,
                            PrManager = new ProfileManager { nameOfProfile = d.uProfile }
                        };

                        userManagers.Add(obj);
                    }
                }

                return userManagers;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return userManagers;
            }
        }

        public UserManager GetUser(string _user, string _pwd)
        {
            UserManager userManager = new UserManager();

            try
            {
                var obj = config.Usrs.Where(u => u.usrname == _user).Where(p => p.usrpassword == _pwd).FirstOrDefault();
                if (obj != null)
                {

                    userManager.Id = obj.Id;
                    userManager.username = obj.usrname;
                    userManager.isActive = obj.isActive == 1 ? @"Yes" : @"No";
                    userManager.isLogged = obj.isLogged == 1 ? @"Yes" : @"No";
                    userManager.nameOfDepartment = new Utility() { }.getDepartment(obj.deptId).Name;
                    userManager.PrManager = new ProfileManager { nameOfProfile = obj.uProfile };
                    userManager.userTag = obj.tag;

                    userManager.sname = obj.surname;
                    userManager.fname = obj.firstname;
                    userManager.onames = obj.othernames;
                }

                return userManager;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return userManager;
            }
        }

        public UserManager GetUser(string _user)
        {
            UserManager userManager = new UserManager();

            try
            {
                var obj = config.Usrs.Where(u => u.usrname == _user).FirstOrDefault();
                if (obj != null)
                {

                    userManager.Id = obj.Id;
                    userManager.username = obj.usrname;
                    userManager.isActive = obj.isActive == 1 ? @"Yes" : @"No";
                    userManager.isLogged = obj.isLogged == 1 ? @"Yes" : @"No";
                    userManager.nameOfDepartment = new Utility() { }.getDepartment(obj.deptId).Name;
                    userManager.PrManager = new ProfileManager { nameOfProfile = obj.uProfile };
                }

                return userManager;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return userManager;
            }
        }

        #endregion

    }

    public struct ProfileManager
    {
        public int Id { get; set; }
        public string nameOfProfile { get; set; }
        public string contentOfProfile { get; set; }
        public string profileInUse { get; set; } //Yes|No
    }

    public struct UserManager
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string nameOfDepartment { get; set; }
        public string isActive { get; set; }
        public string isLogged { get; set; }
        public ProfileManager PrManager { get; set; }

        public string userTag { get; set; }
        public string sname { get; set; }
        public string fname { get; set; }
        public string onames { get; set; }
    }

}