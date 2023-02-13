using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigiProc;
using System.Diagnostics;

namespace DigiProc.Helpers
{
    public interface IPFHelper
    {
        bool CreateCommittee();
        bool CreatePosition();

        bool AddCommitteeMember();
        bool AddNotifier();

        ProcessFlow GetProcessFlow(int _procurement_type_id);
        ProcessFlowList GetProcessFlowList(int _processflowID);

    }
    public class PFHelper: IPFHelper
    {
        #region Properties

        private ProcurementDbEntities config = new ProcurementDbEntities();

        public Committee oCommittee { get; set; }

        public CommitteeMember oCommitteeMember { get; set; }
        public Position oPosition { get; set; }
        public NotificationList oNotifier { get; set; }

        public ProcessFlow oProcessFlow { get; set; }

        public ProcessFlowList oProcessFlowList { get; set; }

        public ProcessFlow GetProcessFlow(int _procurement_type_id)
        {
            ProcessFlow pf = null;

            try
            {
                this.oProcessFlow = config.ProcessFlows.Where(p => p.ProcurementTypeId == _procurement_type_id).FirstOrDefault();
                if (this.oProcessFlow != null)
                {
                    pf = new ProcessFlow() {
                        ProcessFlowID = this.oProcessFlow.ProcessFlowID,
                        ProcurementTypeId = this.oProcessFlow.ProcurementTypeId,
                        Limit = this.oProcessFlow.Limit,
                        ProcessFlowOrder = this.oProcessFlow.ProcessFlowOrder,
                    }; 
                }

                return pf;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return pf;
            }
        }

        public ProcessFlow GetProcessFlow(string _process_flow_id)
        {
            ProcessFlow pf = null;
            int _id = int.Parse(_process_flow_id);

            try
            {
                this.oProcessFlow = config.ProcessFlows.Where(p => p.ProcessFlowID == _id).FirstOrDefault();
                if (this.oProcessFlow != null)
                {
                    pf = new ProcessFlow()
                    {
                        ProcessFlowID = this.oProcessFlow.ProcessFlowID,
                        ProcurementTypeId = this.oProcessFlow.ProcurementTypeId,
                        Limit = this.oProcessFlow.Limit,
                        ProcessFlowOrder = this.oProcessFlow.ProcessFlowOrder,
                    };
                }

                return pf;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return pf;
            }
        }

        public ProcessFlowList GetProcessFlowList(int _processflowID)
        {
            ProcessFlowList pfl = null;

            try
            {
                this.oProcessFlowList = config.ProcessFlowLists.Where(x => x.ProcessFlowID == _processflowID).FirstOrDefault();
                if (this.oProcessFlowList != null)
                {
                    pfl = new ProcessFlowList() {
                        ProcessFlowListID = this.oProcessFlowList.ProcessFlowListID,
                        ProcessFlowID = this.oProcessFlowList.ProcessFlowID,
                        Flow = this.oProcessFlowList.Flow
                    };
                }

                return pfl;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return pfl;
            }
        }

        public List<ProcessFlowList> GetProcessFlowList(string systemUserTag)
        {
            //using the tag of the system user to get associated processflow

            List<ProcessFlowList> pflList = null;

            try
            {
                var dta = config.ProcessFlowLists.Where(x => x.Flow.Contains(systemUserTag)).ToList();
                if (dta.Count() > 0)
                {
                    pflList = new List<ProcessFlowList>();

                    foreach(var d in dta)
                    {
                        var pfl = new ProcessFlowList()
                        {
                            ProcessFlowListID = d.ProcessFlowListID,
                            ProcessFlowID = d.ProcessFlowID,
                            Flow =  d.Flow
                        };

                        pflList.Add(pfl);
                    }

                }

                return pflList;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return pflList;
            }
        }

        #endregion

        #region Implementations

        public bool CreateCommittee()
        {
            //create new record or update existing
            try
            {
                if (oCommittee.CommitteeID == 0)
                {
                    config.Committees.Add(oCommittee);
                }
                else
                {
                    var o = config.Committees.Where(c => c.CommitteeID == oCommittee.CommitteeID).FirstOrDefault();
                    o.CommitteeName = oCommittee.CommitteeName;
                    o.CommitteeDescription = oCommittee.CommitteeDescription;
                }

                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool CreatePosition()
        {
            //create a record or update existing one
            try
            {
                if (oPosition.PositionID == 0)
                {
                    config.Positions.Add(oPosition);
                }
                else
                {
                    var o = config.Positions.Where(p => p.PositionID == oPosition.PositionID).FirstOrDefault();
                    o.Designation = oPosition.Designation;
                }
                
                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                throw ex;
            }
        }

        public bool AddCommitteeMember()
        {
            try
            {
                if (oCommitteeMember.CommitteeMemberID == 0)
                {
                    config.CommitteeMembers.Add(oCommitteeMember);
                }
                else
                {
                    var o = config.CommitteeMembers.Where(c => c.CommitteeMemberID == oCommitteeMember.CommitteeMemberID).FirstOrDefault();
                    o.CommitteeID = oCommitteeMember.CommitteeID;
                    o.LastName = oCommitteeMember.LastName;
                    o.FirstName = oCommitteeMember.FirstName;
                    o.OtherNames = oCommitteeMember.OtherNames;
                    o.PositionID = oCommitteeMember.PositionID;
                    o.EmailAddress = oCommitteeMember.EmailAddress;
                    o.active = oCommitteeMember.active;
                }

                config.SaveChanges();
                return true;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                throw x;
            }
        }

        public bool AddNotifier()
        {
            //create new record or update existing
            try
            {
                if (oNotifier.NotificationID == 0)
                {
                    config.NotificationLists.Add(oNotifier);
                }
                else
                {
                    var o = config.NotificationLists.Where(n => n.NotificationID == oNotifier.NotificationID).FirstOrDefault();
                    o.FirstName = oNotifier.FirstName;
                    o.LastName = oNotifier.LastName;
                    o.EmailAddress = oNotifier.EmailAddress;
                    o.tag = oNotifier.tag;
                    o.isActive = oNotifier.isActive;
                }
                
                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                throw ex;
            }
        }

        #endregion



    }

    public struct GenericLookup
    {
        public int Id { get; set; }
        public string value { get; set; }
    }

}