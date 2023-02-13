using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DigiProc.Helpers;
using DigiProc;

namespace DigiProc.Controllers
{
    public class CommitteeController : Controller
    {
        #region Positions 

        [HttpPost]
        public JsonResult AddPosition(int _id, string cposition)
        {
            try
            {
                var obj = new Position() { 
                    PositionID = _id,
                    Designation = cposition
                };

                var helper = new CommitteeHelper() {  };
                var bln = helper.SavePosition(obj);

                return Json(new { status=bln, data = obj  },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPositions()
        {
            try
            {
                var list = new CommitteeHelper() { }.GetPositions();
                return Json(new { status = true, data = list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Committee

        public JsonResult AddCommittee(int _id, string cName, string cDescrib)
        {
            try
            {
                var obj = new Committee() { 
                    CommitteeID = _id,
                    CommitteeName = cName,
                    CommitteeDescription = cDescrib
                };

                var bln = new CommitteeHelper() { }.SaveCommittee(obj);
                return Json(new { status = bln, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = false, error = $"error: {e.Message}" },JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCommitteeList()
        {
            try
            {
                var committee_list = new CommitteeHelper() { }.GetCommittees();
                return Json(new { status = true, data = committee_list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = false, error = $"error: {e.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Committee-Membership

        [HttpPost]
        public JsonResult AddCommitteeMember(string parameters)
        {
            try
            {
                var parts = parameters.Split(',');

                var obj = new CommitteeMember() { 
                    CommitteeMemberID = int.Parse(parts[0]),
                    FirstName = parts[1],
                    LastName = parts[2],
                    OtherNames = parts[3],
                    EmailAddress = parts[4],
                    PositionID = int.Parse(parts[5]),
                    CommitteeID = int.Parse(parts[6]),
                    active = parts[7] == @"Active"? 1: 0
                };

                bool bln = new CommitteeHelper() { }.SaveCommitteeMembership(obj);
                return Json(new { status = bln, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCommitteeMembers(int cId)
        {
            try
            {
                var _data = new CommitteeHelper() { }.GetCommitteeMembers(cId);
                return Json(new { status = true, data = _data },JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = false, error = $"error: {e.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Procurement-Type

        [HttpGet]
        public JsonResult GetProcurementTypes()
        {
            try
            {
                var procurementTypeList = new CommitteeHelper() { }.GetProcurementTypes();
                return Json(new { status = true, data = procurementTypeList },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveProcurementType(int _id, string procDescrib)
        {
            try
            {
                var helper = new CommitteeHelper() { };
                var obj = new ProcurementType()
                {
                    ProcurementTypeID = _id,
                    ProcurementDescription = procDescrib
                };

                bool bln = helper.SaveProcurementType(obj);
                return Json(new { status = true, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new {status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region NotificationGroups

        [HttpPost]
        public JsonResult SaveNotificationGroup(int _id, string gname, string gemails, string gdescrib)
        {
            try
            {
                var obj = new NotificationGroup() { 
                    NotificationGroupID = _id,
                    NotificationGroupName = gname,
                    NotificationMailString = gemails,
                    NotificationDescription = gdescrib
                };

                bool bln = new CommitteeHelper() { }.SaveNotificationGroup(obj);
                return Json(new { status = bln, data = obj }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNotificationGroups()
        {
            try
            {
                var notification_groups = new CommitteeHelper() { }.GetNotificationGroups();
                return Json(new { status = true, data = notification_groups },JsonRequestBehavior.AllowGet);
            }
            catch(Exception x)
            {
                return Json(new { status = false, error = $"error: {x.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Process-Flow

        [HttpPost]
        public JsonResult SaveProcessFlow(int _id, int ptypeId, decimal? limit, int? _order)
        {
            try
            {
                var pfObj = new ProcessFlow() { 
                    ProcessFlowID = _id,
                    ProcurementTypeId = ptypeId,
                    Limit = limit,
                    ProcessFlowOrder = _order
                };

                bool bln = new CommitteeHelper() { }.SaveProcessFlow(pfObj);
                return Json(new { status = true, data =pfObj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProcessFlows()
        {
            try
            {
                var helper = new CommitteeHelper() { };
                var list = helper.GetProcessFlows();

                return Json(new { status = true, data = list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region PFNotificationList

        [HttpPost]
        public JsonResult SavePFNotificationList(int _id, int pfId, int groupId)
        {
            try
            {
                var helper = new CommitteeHelper() { };
                var obj = new PFNotificationList() { 
                    PFNotificationListID = _id,
                    PFID = pfId,
                    NotificationGroupID = groupId
                };

                bool bln = helper.SavePFNotificationList(obj);

                return Json(new { status = bln, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPFNotificationList()
        {
            try
            {
                var list = new CommitteeHelper() { }.GetPFNotifications();
                return Json(new { status = true, data = list },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}" },JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region ProcessFlowList

        [HttpGet]
        public JsonResult GetProcessFlowList(int _processflowID)
        {
            try
            {
                var helper = new CommitteeHelper() { };
                var obj = helper.GetProcessFlowList(_processflowID);
                return Json(new { status = true, data = obj },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, error = $"error: {ex.Message}"},JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}