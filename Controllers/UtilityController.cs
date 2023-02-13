using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Diagnostics;
using DigiProc.Helpers;

namespace DigiProc.Controllers
{
    public class UtilityController : Controller
    {
        [HttpGet]
        public JsonResult GetAssignedModules()
        {
            //return Json(new { });
            //system uses the username session variable to get the modules assigned to this user
            try
            {
                UserSession session = (UserSession)Session["userSession"];
              
                var obj = new Utility();
                var usModules = obj.getUserModules(session.userName);
                return Json(usModules, JsonRequestBehavior.AllowGet);
            }
            catch (Exception errmsg) { Debug.Print(errmsg.Message); return Json(new { error = errmsg }); }
        }

        [HttpGet]
        public JsonResult GetItemCategories()
        {
            try
            {
                var d = new Utility { }.getItemCategories();
                return Json(new { status = true, data = d }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception err) { Debug.Print(err.Message); return Json(new { error = err }); }
        }

        [HttpGet]
        public JsonResult GetSIUnits()
        {
            try
            {
                var item_measurements = new Utility { }.getItemMetrics();
                return Json(new { status = true, data = item_measurements },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex) { Debug.Print(ex.Message); return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet); }
        }

        [HttpGet]
        public JsonResult GetCompanyTypes()
        {
            try
            {
                var company_types =  new Utility { }.getBusinessTypes();
                return Json(new { status = true, data = company_types },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x) { return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet); }
        }

        [HttpGet]
        public JsonResult getAccountPeriods()
        {
            try
            {
                var current_acc_periods = new Utility { }.getFinancialYearsAsync();
                return Json(new { status = true, data = current_acc_periods },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x) { return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet); }
        }

        [HttpGet]
        public JsonResult GetCurrencies()
        {
            try
            {
                var currency_list = new Utility { }.getCurrencies();
                return Json(new { status = true, data = currency_list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x) { return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet); }
        }

        [HttpGet]
        public JsonResult GetDepartments()
        {
            try
            {
                var department_data = new Utility() { }.GetDepartments();
                return Json(new { status = true, data = department_data },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #region Vendor - Types

        [HttpGet]
        public JsonResult GetVendorTypes()
        {
            try
            {
                var vendor_type_list = new Utility() { }.getVendorTypes();
                return Json(new { status = true, data = vendor_type_list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Items

        public JsonResult GetItemCode(string category, int categoryID)
        {
            //uses the category and categoryID to generate the next item code for an item
            try
            {
                var retString = new Utility() { }.generateItemCode(category, categoryID);
                return Json(new { status = true, data = retString },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetItemList()
        {
            //criteria: determines if the items selected should be filtered down (* means ALL)
            //flag: determines the kind of items to select. not used when criteria=*
            try
            {
                var dta = new Utility() { }.GetItems();
                return Json(new { status = true, data = dta },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        

        #endregion

        #region Vendor

        [HttpGet]
        public JsonResult GenerateVendorNo()
        {
            try
            {
                string vendorNo = new VendorHelper() { }.generateVendorID();
                return Json(new { status = true, data = vendorNo },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetVendors()
        {
            try
            {
                var helper = new Utility() { };
                var vendor_data = helper.GetVendors();
                return Json(new { status = true, data = vendor_data },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Priority-Types

        [HttpGet]
        public JsonResult GetPriorityTypes()
        {
            try
            {
                var _data = new Utility() { }.GetPriorityTypes();
                return Json(new { status = true, data = _data },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Requisition-Types

        [HttpGet]
        public JsonResult GetRequisitionTypes()
        {
            try
            {
                var requisition_types = new Utility() { }.GetRequisitionTypes();
                return Json(new { status = true, data = requisition_types },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Procurement-Type

        [HttpGet]
        public JsonResult GetProcurementTypes()
        {
            try
            {
                var procurement_type_list = new Utility { }.GetProcurementTypes();
                return Json(new { status = true, data = procurement_type_list }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error = {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

    }
}