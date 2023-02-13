using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DigiProc;
using System.Diagnostics;
using System.Collections;

namespace DigiProc.Helpers
{
    public interface ICommitteeHelper
    {
        bool SavePosition(Position oPosition);
    }

    public class CommitteeHelper: ICommitteeHelper
    {
        private ProcurementDbEntities config = new ProcurementDbEntities();

        public Position oPosition { get; set; }

        #region Positions

        public bool SavePosition(Position oPosition)
        {
            try
            {
                if (oPosition.PositionID == 0)
                {
                    config.Positions.Add(oPosition);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var o = config.Positions.Where(x => x.PositionID == oPosition.PositionID).FirstOrDefault();
                    o.Designation = oPosition.Designation;
                    config.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public List<Position> GetPositions()
        {
            List<Position> position_list = new List<Position>();
            try
            {
                var dta = config.Positions.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var p = new Position()
                        {
                            PositionID = d.PositionID,
                            Designation = d.Designation
                        };

                        position_list.Add(p);
                    }
                }

                return position_list.ToList();
            }
            catch(Exception ex)
            {
                return position_list;
            }
        }

        public Position GetPosition(int? _id)
        {
            Position obj = null;

            try
            {
                obj = config.Positions.Where(p => p.PositionID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region Committee

        public bool SaveCommittee(Committee o)
        {
            try
            {
                if (o.CommitteeID == 0)
                {
                    config.Committees.Add(o);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var c = config.Committees.Where(cc => cc.CommitteeID == o.CommitteeID).FirstOrDefault();
                    c.CommitteeName = o.CommitteeName;
                    c.CommitteeDescription = o.CommitteeDescription;

                    config.SaveChanges();
                    return true;
                }
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
        }

        public List<Committee> GetCommittees()
        {
            //get committee list
            List<Committee> committee_list = new List<Committee>();

            try
            {
                var cList = config.Committees.ToList();
                if (cList.Count() > 0)
                {
                    foreach(var c in cList)
                    {
                        var o = new Committee()
                        {
                            CommitteeID = c.CommitteeID,
                            CommitteeName = c.CommitteeName,
                            CommitteeDescription = c.CommitteeDescription
                        };

                        committee_list.Add(o);
                    }
                }

                return committee_list.ToList();
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return committee_list;
            }
        }

        public Committee GetCommittee(int? _id)
        {
            Committee obj = null;
            try
            {
                obj = config.Committees.Where(x => x.CommitteeID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region Committee-Membership

        public bool SaveCommitteeMembership(CommitteeMember cm)
        {
            try
            {
                if(cm.CommitteeMemberID == 0)
                {
                    config.CommitteeMembers.Add(cm);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var o = config.CommitteeMembers.Where(x => x.CommitteeMemberID == cm.CommitteeMemberID).FirstOrDefault();
                    if (o != null)
                    {
                        o.CommitteeID = cm.CommitteeID;
                        o.LastName = cm.LastName;
                        o.FirstName = cm.FirstName;
                        o.OtherNames = cm.OtherNames;
                        o.PositionID = cm.PositionID;
                        o.EmailAddress = cm.EmailAddress;
                        o.active = cm.active;

                        config.SaveChanges();
                        return true;
                    }
                    else { return false; }
                }
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
        }

        public List<Membership> GetCommitteeMembers(int committeeId)
        {
            //List<CommitteeMember> member_list = new List<CommitteeMember>();
            List<CommitteeMember> list = new List<CommitteeMember>();
            List<Membership> members = new List<Membership>();
            var helper = new CommitteeHelper();

            try
            {
                if (committeeId == 0)
                {
                    list = config.CommitteeMembers.ToList();
                }
                else
                {
                    list = config.CommitteeMembers.Where(c => c.CommitteeID == committeeId).ToList();
                }
                
                if (list.Count() > 0)
                {
                    foreach(var item in list)
                    {
                        var m = new Membership()
                        {
                            Id = item.CommitteeMemberID,
                            nameOfCommittee = helper.GetCommittee(item.CommitteeID).CommitteeName,
                            firstname = item.FirstName,
                            surname = item.LastName,
                            othernames = item.OtherNames,
                            nameOfposition = helper.GetPosition(item.PositionID).Designation,
                            email = item.EmailAddress,
                            actStatus = item.active == 1? @"Active": @"Inactive"
                        };

                        members.Add(m);
                    }
                }

                return members;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return members;
            }
        }

        #endregion

        #region ProcurementType

        public bool SaveProcurementType(ProcurementType item)
        {
            try
            {
                if (item.ProcurementTypeID == 0)
                {
                    config.ProcurementTypes.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var obj = config.ProcurementTypes.Where(p => p.ProcurementTypeID == item.ProcurementTypeID).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.ProcurementDescription = item.ProcurementDescription;
                        config.SaveChanges();
                        return true;
                    }
                    else { return false; }
                }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public List<ProcurementType> GetProcurementTypes()
        {
            var procurement_types = new List<ProcurementType>();
            try
            {
                var list = config.ProcurementTypes.ToList();
                if (list.Count() > 0)
                {
                    foreach(var item in list)
                    {
                        var p = new ProcurementType() { 
                            ProcurementTypeID = item.ProcurementTypeID,
                            ProcurementDescription = item.ProcurementDescription
                        };

                        procurement_types.Add(p);
                    }
                }

                return procurement_types;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return procurement_types;
            }
        }

        public ProcurementType GetProcurementType(int? _id)
        {
            ProcurementType obj = null;
            try
            {
                obj = config.ProcurementTypes.Where(p => p.ProcurementTypeID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region PFNotificationList

        public bool SavePFNotificationList(PFNotificationList item)
        {
            try
            {
                if (item.PFNotificationListID == 0)
                {
                    config.PFNotificationLists.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var o = config.PFNotificationLists.Where(pf => pf.PFNotificationListID == item.PFNotificationListID).FirstOrDefault();
                    if (o != null)
                    {
                        o.PFID = item.PFID;
                        o.NotificationGroupID = item.NotificationGroupID;
                        config.SaveChanges();

                        return true;
                    }
                    else { return false; }
                }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public List<PFNotificationLookup> GetPFNotifications()
        {
            List<PFNotificationLookup> list = new List<PFNotificationLookup>();
            try
            {
                var dta = config.PFNotificationLists.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var obj = new PFNotificationLookup() { 
                            Id = d.PFNotificationListID,
                            IdOfProcessFlow = d.PFID,
                            nameOfGroup = this.GetNotificationGroup(d.NotificationGroupID).NotificationGroupName
                        };

                        list.Add(obj);
                    }
                }

                return list;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return list;
            }
        }

        #endregion

        #region Notification Group

        public bool SaveNotificationGroup(NotificationGroup item)
        {
            try
            {
                if (item.NotificationGroupID == 0)
                {
                    config.NotificationGroups.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var obj = config.NotificationGroups.Where(ng => ng.NotificationGroupID == item.NotificationGroupID).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.NotificationGroupName = item.NotificationGroupName;
                        obj.NotificationMailString = item.NotificationMailString;
                        obj.NotificationDescription = item.NotificationDescription;

                        config.SaveChanges();
                        return true;
                    }
                    else { return false; }
                }
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public List<NotificationGroup> GetNotificationGroups()
        {
            List<NotificationGroup> groups = new List<NotificationGroup>();
            try
            {
                var list = config.NotificationGroups.ToList();
                if (list.Count() > 0)
                {
                    foreach(var item in list)
                    {
                        var ob = new NotificationGroup() { 
                            NotificationGroupID = item.NotificationGroupID,
                            NotificationGroupName = item.NotificationGroupName,
                            NotificationMailString = item.NotificationMailString,
                            NotificationDescription = item.NotificationDescription
                        };

                        groups.Add(ob);
                    }
                }

                return groups;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return groups;
            }
        }

        public NotificationGroup GetNotificationGroup(int? _id)
        {
            NotificationGroup obj = null;

            try
            {
                obj = config.NotificationGroups.Where(ng => ng.NotificationGroupID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region ProcessFlow

        public bool SaveProcessFlow(ProcessFlow item)
        {
            try
            {
                if (item.ProcessFlowID == 0)
                {
                    config.ProcessFlows.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var o = config.ProcessFlows.Where(pf => pf.ProcessFlowID == item.ProcessFlowID).FirstOrDefault();
                    if (o != null)
                    {
                        o.ProcurementTypeId = item.ProcurementTypeId;
                        o.Limit = item.Limit;
                        o.ProcessFlowOrder = item.ProcessFlowOrder;
                        config.SaveChanges();
                        return true;
                    }
                    else { return false; }
                }
            }
            catch(Exception x)
            {
                return false;
            }
        }

        public List<PFLookup> GetProcessFlows()
        {
            List<PFLookup> list = new List<PFLookup>();
            try
            {
                var dta = config.ProcessFlows.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var pf = new PFLookup() { 
                            Id = d.ProcessFlowID,
                            nameOfProcurement = this.GetProcurementType(d.ProcurementTypeId).ProcurementDescription,
                            limit = d.Limit,
                            order = d.ProcessFlowOrder
                        };

                        list.Add(pf);
                    }
                }

                return list;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return list;
            }
        }
        
        public PFLookup GetProcessFlow(int _id)
        {
            ProcessFlow obj = null;
            PFLookup res = new PFLookup();

            try
            {
                obj = config.ProcessFlows.Where(pf => pf.ProcessFlowID == _id).FirstOrDefault();

                if (obj != null)
                {

                    res.Id = obj.ProcessFlowID;
                    res.nameOfProcurement = this.GetProcurementType(obj.ProcurementTypeId).ProcurementDescription;
                    res.limit = obj.Limit;
                    res.order = obj.ProcessFlowOrder;
                    
                    return res;
                }
                else { return res; }
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return res;
            }
        }

        #endregion

        #region FlowList

        public ProcessFlowList GetProcessFlowList(int pflowId)
        {
            ProcessFlowList obj = null;

            try
            {
                obj = config.ProcessFlowLists.Where(pf => pf.ProcessFlowID == pflowId).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

    }

    public struct Membership
    {
        public int Id { get; set; }
        public string nameOfCommittee { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string othernames { get; set; }
        public string nameOfposition { get; set; }
        public string email { get; set; }
        public string actStatus { get; set; }
    }

    public struct PFLookup
    {
        public int Id { get; set; }
        public string nameOfProcurement { get; set; }
        public decimal? limit { get; set; }
        public int? order { get; set; }
    }

    public struct PFNotificationLookup
    {
        public int Id { get; set; }
        public int IdOfProcessFlow { get; set; }
        public string nameOfGroup { get; set; }
    }

}