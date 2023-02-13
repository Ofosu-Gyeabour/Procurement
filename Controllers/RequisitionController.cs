using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DigiProc;
using DigiProc.Helpers;
using System.Diagnostics;

using System.IO;

namespace DigiProc.Controllers
{
    public class RequisitionController : Controller
    {
        //private UserSession session = new UserSession();

        public RequisitionController()
        {
            //session = (UserSession)Session["userSession"];
        }

        [HttpGet]
        public JsonResult GetRequisitionPrelimData()
        {
            try
            {
                UserSession session = (UserSession)Session["userSession"];
                var strRequisitionNo = new RequisitionHelper() { }.GenerateRequisitionNumber(session.userDepartment.Name);
                var obj = new Utility();
                var defaultCompany = obj.getDefaultCompany();
                
                return Json(new { status = true, reqNo = strRequisitionNo, 
                    requester = session.userName, department = session.userDepartment.Name, 
                    companyName = defaultCompany.CompanyDescription },JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRequisitionDetails(int requisitionID)
        {
            try
            {
                var _details = new RequisitionHelper() { }.getRequisitionDetails(requisitionID);
                return Json(new { status = true, data = _details },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRequisitionDetails2(int requisitionID, int statusID)
        {
            try
            {
                var _details = new RequisitionHelper() { }.getRequisitionDetails(requisitionID,statusID);
                //var requisition_items = new RequisitionHelper() { }.GetRequisitionItemLookups(requisitionID, statusID);

                return Json(new { status = true, data = _details /*, items = requisition_items */ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRequisitionNos(int departmentID, int statusID)
        {
            try
            {
                var requisition_numbers = new RequisitionHelper() { }.getRequisitionNumbers(departmentID,statusID);
                return Json(new { status = true, data = requisition_numbers },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PostRequisitionRequest(string rqNo, string requestee, string comp, string dept,
                                                    int? rqnId, int? currencyId, int? priorityId, string location,string comment, string[] values)
        {
            try
            {
                //insert into dbo.requisition
                //insert into dbo.requisitionitems
                var util = new Utility();
                var oCompany = util.getDefaultCompany();
                var oDepartment = util.getDepartment(dept);
                var oFin = util.getActiveFinancialYear();
                var oReqStatus = util.GetRequisitionStatus(@"Pending");
                UserSession session = (UserSession) Session["userSession"];

                int success = 0;
                int failed = 0;

                var o = new Requisition() 
                { 
                    RequisitionNo = rqNo,
                    RequestedBy = requestee,
                    FinancialYrID = oFin.FinancialYrID,
                    CompanyID = oCompany.CompanyID,
                    DepartmentID = oDepartment.DepartmentID,
                    RequisitionTypeID = rqnId,
                    Location = location,
                    CurrencyID = currencyId,
                    PriorityID = priorityId,
                    RequisitionDescription = comment,
                    RequisitionStatusID = oReqStatus.RequisitionStatusID,
                    CreatedBy = session.userName,
                    CreatedDate = DateTime.Now,
                    isNotif = 0
                };

                var helper = new RequisitionHelper();
                var requisition_id = helper.SaveRequisition(o);

                if (requisition_id > 0)
                {
                    //iteration
                    foreach(var value in values)
                    {
                        //split
                        var str = value.Split(',');

                        //create object
                        var oReqItem = new RequisitionItem() { 
                            RequisitionID = requisition_id,
                            ItemID = new Utility() { }.getItemID(str[0]),
                            Narration = str[1],
                            Quantity = Convert.ToInt32(str[2]),
                            FinApprovalStatus = 1  //PENDING
                        };

                        if (helper.SaveRequisitionItems(oReqItem)) { success += 1; } else { failed += 1; }
                    }
                }

                return Json(new { status = true, data = $"Requisition {rqNo} saved successfully\r\nTotal Count = {values.Length.ToString()}, Successful inserts = {success.ToString()} Failed inserts = {failed.ToString()}" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception exc)
            {
                return Json(new { status = false, error = $"error: {exc.Message}" },JsonRequestBehavior.AllowGet);
            }
        }
    
    
        [HttpGet]
        public JsonResult GetRequisitionItemList(int requisitionID, int statusID)
        {
            try
            {
                var requisition_items = new RequisitionHelper() { }.GetRequisitionItemLookups(requisitionID, statusID);
                return Json(new { status = true, data = requisition_items },JsonRequestBehavior.AllowGet);
            }
            catch(Exception exc)
            {
                return Json(new { status = false, error = $"error: {exc.Message}"},JsonRequestBehavior.AllowGet);
            }
        }
    
        [HttpGet]
        public JsonResult GetRequisitionItemListUsingLPO(int LocalPONumber)
        {
            try
            {
                var requisition_items = new RequisitionHelper { }.GetRequisitionItemLookups(LocalPONumber);
                return Json(new { status = true, data = requisition_items },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, data = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]        
        public JsonResult ApproveRequisition(string[] dta)
        {
            try
            {
                int success = 0;
                int failed = 0;
                bool bln = false;

                int req_id = 0;

                if (dta.Length > 0)
                {
                    RequisitionHelper Cfg = new RequisitionHelper();

                    foreach (var d in dta)
                    {
                        var str = d.Split(',');
                        req_id = int.Parse(str[1]);

                        var obj = new RequisitionItem() { RequisitionItemID = int.Parse(str[0]) };
                        bln = Cfg.ApproveRequisitionItem(obj.RequisitionItemID, 2);



                        if (bln) { success += 1; } else { failed += 1; }
                    }

                    if (success == dta.Length)
                    {
                        //amend the status of the requisition to approved
                        Cfg.ChangeRequisitionStatus(req_id, 2);  //2 = financial approval
                    }

                    //if (success == dta.Length) { new RequisitionHelper { }.ChangeRequisitionStatus(req_id, 2); }
                }

                return Json(new { status = bln, data = $"{success.ToString()} requisition items approved" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CreateLPORecord(string rNo, int vID, int LPOStatus,int finStatusId,decimal totAmt, string[] dta)
        {
            try
            {
                var helper = new RequisitionHelper { };
                bool blnSuccess = false;

                //update requisition items
                if (dta.Length > 0)
                {
                    int success = 0; int failed = 0;

                    var LPONum = new RequisitionHelper() { }.GetLPOCount(rNo.Replace("REQN", "LPO"));
                    
                    var objPurchaseOrder = new LPO() { RequisitionNo = rNo, VendorID = vID, LPOStatusID = LPOStatus, TotAmt = totAmt, LPONo = LPONum };
                    int _id = helper.SaveLPO(objPurchaseOrder);

                    if (_id > 0)
                    {
                        foreach (var d in dta)
                        {
                            var str = d.Split(',');
                            var rq = new RequisitionItem() { RequisitionItemID = int.Parse(str[0]), Amt = decimal.Parse(str[1]),FinApprovalStatus = finStatusId, LPOID = _id };

                            blnSuccess = helper.UpdateRequisitionItem(rq);
                            if (blnSuccess) { success += 1; } else { failed += 1; }
                        }
                    }

                    if (success == dta.Length) {
                        new RequisitionHelper() { }.ChangeRequisitionStatus(rNo, LPOStatus);
                    }
                }

                return Json(new { status = blnSuccess, data = $"Local Purchasing Order record created for {dta.Length.ToString()} items" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, data = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetLPOs()
        {
            //gets distinct LPOs
            try
            {
                //var POs = new RequisitionHelper { }.GetDistinctLPORequisitionNumbers();
                var POs = new RequisitionHelper { }.GetLocalPurchaseOrders();
                return Json(new { status = true, data = POs },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //public JsonResult CreateLocalPurchasingOrder(int _id, string vat,DateTime pdate, DateTime expdate, string shipping, string payment, string terms)
        public JsonResult CreateLocalPurchasingOrder(string _value)
        {
            try
            {
                var obj = new LPO() { };

                var _values = _value.Split(',');

                obj.LPOID = int.Parse(_values[0]);
                obj.VAT = decimal.Parse(_values[1]);
                obj.PurchaseOrderDate = DateTime.ParseExact(_values[2],"dd-MM-yyyy", null);
                obj.ExpectedDeliveryDate = DateTime.ParseExact(_values[3], "dd-MM-yyyy", null);
                obj.ShippingAddress = _values[4];
                obj.PaymentTerm = _values[5];
                obj.OtherTermsAndConditions = _values[6];
                obj.ProcurementTypeId = int.Parse(_values[8]);
                obj.LPOStatusID = 4; //4=LPO Generated

                var bln = new RequisitionHelper { }.SaveLocalPurchaseOrder(obj);
                return Json(new { status = bln, data = obj },JsonRequestBehavior.AllowGet);  
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProcessFlowDetails(int _procurement_type_id)
        {
            //uses procurement type id to fetch threshold amount, and the list of personnel to authorize procurement
            try
            {
                var pfObj = new PFHelper() { }.GetProcessFlow(_procurement_type_id);
                ProcessFlowList pfl = null;
                List<GenericLookup> nameList = new List<GenericLookup>();

                if (pfObj != null)
                {
                    //get process flow list using processflowid
                    pfl = new PFHelper() { }.GetProcessFlowList(pfObj.ProcessFlowID);
                    string[] str = pfl.Flow.Split('|');

                    //iteration
                    int k = 1;
                    foreach(var s in str)
                    {
                        var postBody = new GenericLookup { 
                            Id = k,
                            value = s
                        };

                        nameList.Add(postBody);
                    }

                    return Json(new { status = true, data = nameList, limit = pfObj.Limit },JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { status = false, data=@"no data" },JsonRequestBehavior.AllowGet); }
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FileUpload()
        {
            try
            {
                int i = Request.Files.Count;
                HttpPostedFileBase postedFile = Request.Files[0];
                Stream input = postedFile.InputStream;
                byte[] inputByte;

                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    inputByte = ms.ToArray();

                    return Json(true);
                }

            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

    }
}