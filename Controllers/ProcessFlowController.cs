using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigiProc;
using DigiProc.Helpers;

namespace DigiProc.Controllers
{
    public class ProcessFlowController : Controller
    {

        #region Committee

        public JsonResult SaveCommittee(int id, string cname, string cDescription)
        {
            try
            {
                var obj = new Committee() 
                {
                    CommitteeName = cname,
                    CommitteeDescription = cDescription
                };

                bool bln = new PFHelper() { oCommittee = obj }.CreateCommittee();
                return Json(new { status = bln, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Position

        public JsonResult SavePosition(int Id, string designation)
        {
            try
            {
                var wrapper = new PFHelper() { 
                    oPosition = new Position()
                    {
                        Designation = designation
                    }
                };

                bool bln = wrapper.CreatePosition();
                return Json(new { status = bln, data = wrapper.oPosition },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}"},JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region CommitteeMembership

        public JsonResult SaveCommitteeMembership(int memberID, int cID, string lastname, string firstname, string othernames, int positionID, string email, int isActive )
        {
            try
            {
                var obj = new CommitteeMember() { 
                    CommitteeMemberID = memberID,
                    CommitteeID = cID,
                    LastName = lastname,
                    FirstName = firstname,
                    OtherNames = othernames,
                    PositionID = positionID,
                    EmailAddress = email,
                    active = isActive
                };

                var wrapper = new PFHelper() { oCommitteeMember = obj };
                return Json(new { status = wrapper.AddCommitteeMember(), data = wrapper.oCommitteeMember },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region NotificationList

        public JsonResult AddNotificationList(int notifID, string firstname, string lastname, string email, string ntag, int active)
        {
            try
            {
                var obj = new NotificationList() { 
                    NotificationID = notifID,
                    FirstName = firstname,
                    LastName = lastname,
                    EmailAddress = email,
                    tag = ntag,
                    isActive = active
                };

                var helper = new PFHelper() { oNotifier = obj };
                return Json(new { status = helper.AddNotifier(), data = helper.oNotifier },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        [HttpGet]
        public JsonResult GetLPOsToApprove()
        {
            //using the tag of the system user, get the appropriate LPOs for him/her to approve
            try
            {
                var session = (UserSession)Session["userSession"];
                var Cfg = new PFHelper();
                var rCfg = new RequisitionHelper();

                List<ProcessFlow> opf = new List<ProcessFlow>();
                List<LocalPurchaseOrderLookup> results = new List<LocalPurchaseOrderLookup>();

                var process_flow_list = Cfg.GetProcessFlowList(session.approverTag);

                if (process_flow_list.Count() > 0)
                {
                    foreach(var pf in process_flow_list)
                    {
                        var obj = Cfg.GetProcessFlow(pf.ProcessFlowID.ToString());
                        if (obj != null)
                        {
                            opf.Add(obj);
                        }
                    }

                    //iterate over opf list variable to get the list of LPOs
                    foreach(var op in opf)
                    {
                        var x = rCfg.GetLocalPurchaseOrders(op.ProcurementTypeId);
                        if (x.Count() > 0)
                        {
                            foreach(var xo in x)
                            {
                                results.Add(xo);
                            }
                        }
                    }
                }

                return Json(new { status = true, data = results },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetLPOApprovalHistory(int LPO_ID)
        {
            //gets the records of approvals for a specific LPO
            try
            {
                var Cfg = new RequisitionHelper();
                var approvalHistory = Cfg.GetLPOApprovalLookups(LPO_ID);

                return Json(new { status = true, data = approvalHistory },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveApprovalActivity(int _ID, string _status, string _comments)
        {
            try
            {
                var session = (UserSession)Session["userSession"];
                var Cfg = new RequisitionHelper();

                var obj = new LPOApproval() 
                { 
                    LPO_ID = _ID,
                    PersonTag = session.approverTag,
                    ApprovalDate = DateTime.Now,
                    ApprovalStatus = _status == @"Approve"? 1: 0,
                    ApprovalComments = _comments,
                    PersonName = session.bioName
                };

                bool bln = Cfg.SaveLPOApproval(obj);
                if (bln)
                {
                    return Json(new { status = bln, data = $"{obj.PersonTag} has voted to {_status} procurement." }, JsonRequestBehavior.AllowGet);
                }
                else { return Json(new { status = false, data = @"Approval could not be saved.Please contact the Administrator" },JsonRequestBehavior.AllowGet); }
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

    }
}