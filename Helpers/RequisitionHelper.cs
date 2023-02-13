using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DigiProc;
using System.Diagnostics;


namespace DigiProc.Helpers
{
    public class RequisitionHelper
    {
        private ProcurementDbEntities config = new ProcurementDbEntities();

        #region Requisition
        public string GenerateRequisitionNumber(string dept)
        {
            var dateStr = string.Format("{0}-{1}-{2}",@"REQN",dept, getDate());
            int _id = GetRequisitionCounter(dateStr);

            string _idString = new VendorHelper() { }.formatVendorID(_id.ToString());
            return string.Format("{0}-{1}", dateStr, _idString);
        }

        private string getDate()
        {
            var s = DateTime.Now.ToString("dd-MM-yyyy");
            var d = s.Substring(0, 2);
            var m = s.Substring(3, 2);
            var y = s.Substring(6, 4);

            return String.Format("{0}{1}{2}", y, m, d);
        }

        private int GetRequisitionCounter(string param)
        {
            //gets the counter for the requisition
            try
            {
                int _id = config.Requisitions.Where(x => x.RequisitionNo.Contains(param)).ToList().Count();
                return (_id + 1);
            }
            catch(Exception x)
            {
                return 1;
            }
        }

        public int SaveRequisition(Requisition item)
        {
            try
            {
                config.Requisitions.Add(item);
                config.SaveChanges();
                return item.RequisitionID;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return 0;
            }
        }

        public RequisitionLookup getRequisitionDetails(int _id)
        {
            //gets readonly data for requisitions
            RequisitionLookup obj = new RequisitionLookup();

            var helper = new Utility() { };

            try
            {
                var o = config.Requisitions.Where(r => r.RequisitionID == _id).FirstOrDefault();
                if (o != null)
                {
                    obj.Id = o.RequisitionID;
                    obj.RequisitionNo = o.RequisitionNo;
                    obj.Requestee = o.RequestedBy;
                    obj.nameOfDepartment = helper.getDepartment(o.DepartmentID).Name;
                    obj.priority = helper.GetPriority(o.PriorityID).PriorityDescription;
                }

                return obj;
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                return obj;
            }
        }

        public RequisitionLookup getRequisitionDetails(int _id, int _status)
        {
            //gets readonly data for requisitions
            RequisitionLookup obj = new RequisitionLookup();

            var helper = new Utility() { };

            try
            {
                var o = config.Requisitions.Where(r => r.RequisitionID == _id).FirstOrDefault();
                if (o != null)
                {
                    obj.Id = o.RequisitionID;
                    obj.RequisitionNo = o.RequisitionNo;
                    obj.Requestee = o.RequestedBy;
                    obj.nameOfDepartment = helper.getDepartment(o.DepartmentID).Name;
                    obj.priority = helper.GetPriority(o.PriorityID).PriorityDescription;
                    obj.rLookups = this.GetRequisitionItemLookups(_id, _status);
                }

                return obj;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return obj;
            }
        }

        public List<RequisitionLookup> getRequisitionNumbers(int departmentId, int statusId)
        {
            List<RequisitionLookup> list = new List<RequisitionLookup>();
            try
            {
                //var dta = config.Requisitions.ToList();
                var dta = config.Requisitions.Where(rq => rq.DepartmentID == departmentId).Where(r => r.RequisitionStatusID == statusId).ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var o = new RequisitionLookup() { 
                            Id = d.RequisitionID,
                            RequisitionNo = d.RequisitionNo,
                            Requestee = d.RequestedBy,
                            priority = new Utility() { }.GetPriority(d.PriorityID).PriorityDescription
                        };

                        list.Add(o);
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

        public bool ChangeRequisitionStatus(string requisitionNo, int _statusID)
        {
            //change the requisition status using requisition and status id 
            try
            {
                var obj = config.Requisitions.Where(r => r.RequisitionNo == requisitionNo).FirstOrDefault();
                obj.RequisitionStatusID = _statusID;

                config.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }
        public bool ChangeRequisitionStatus(int _requisitionID, int _statusID)
        {
            //change the requisition status using requisition and status id 
            try
            {
                var obj = config.Requisitions.Where(r => r.RequisitionID == _requisitionID).FirstOrDefault();
                obj.RequisitionStatusID = _statusID;

                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        #endregion

        #region RequisitionItems
        public bool SaveRequisitionItems(RequisitionItem obj)
        {
            //saves requisition items
            try
            {
                config.RequisitionItems.Add(obj);
                config.SaveChanges();
                return true;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
        }

        public List<reqItem> GetRequisitionItemLookups(int reqId, int statusId)
        {
            //use requisitionId to fetch requisition items
            List<reqItem> rqitems = new List<reqItem>();
            try
            {
                var dta = config.RequisitionItems.Where(rq => rq.RequisitionID == reqId).Where(r => r.FinApprovalStatus == statusId).ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var obj = new Utility { }.GetItem((int)d.ItemID);

                        var o = new reqItem() { 
                            Id = d.RequisitionItemID,
                            RequisitionId = (int) d.RequisitionID,
                            ProductCode = obj.ProductCode,
                            ProductName = obj.ProductName,
                            Quantity = (int) d.Quantity,
                            narration = d.Narration
                        };

                        rqitems.Add(o);
                    }
                }

                return rqitems;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return rqitems;
            }
        }

        public List<RequisitionItemLookup> GetRequisitionItemLookups(int LPO_ID)
        {
            //use requisitionId to fetch requisition items
            List<RequisitionItemLookup> rqitems = new List<RequisitionItemLookup>();
            try
            {
                var dta = config.RequisitionItems.Where(rq => rq.LPOID == LPO_ID).ToList();
                if (dta.Count() > 0)
                {
                    foreach (var d in dta)
                    {
                        var o = new RequisitionItemLookup()
                        {
                            Id = d.RequisitionItemID,
                            RequisitionId = (int)d.RequisitionID,
                            item = new Utility { }.GetItem((int)d.ItemID),
                            Quantity = (int)d.Quantity,
                            narration = d.Narration,
                            amt = d.Amt
                        };

                        rqitems.Add(o);
                    }
                }

                return rqitems;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return rqitems;
            }
        }

        public bool ApproveRequisitionItem(int _id,int _statusId)
        {
            //method is responsible for the approval of a requisition item
            try
            {
                var obj = config.RequisitionItems.Where(rq => rq.RequisitionItemID == _id).FirstOrDefault();
                if (obj != null)
                {
                    obj.FinApprovalStatus = _statusId;
                    config.SaveChanges();

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

        public bool UpdateRequisitionItem(RequisitionItem item)
        {
            //method updates requisition item
            try
            {
                var obj = config.RequisitionItems.Where(r => r.RequisitionItemID == item.RequisitionItemID).FirstOrDefault();
                if (obj != null)
                {
                    obj.Amt = item.Amt;
                    obj.FinApprovalStatus = item.FinApprovalStatus;
                    obj.LPOID = item.LPOID;

                    config.SaveChanges();
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

        #endregion

        #region Local Purchase Order - LPO

        public bool SaveLocalPurchaseOrder(LPO item)
        {
            try
            {
                if (item.LPOID == 0)
                {
                    config.LPOes.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var obj = config.LPOes.Where(x => x.LPOID == item.LPOID).FirstOrDefault();
                    obj.VAT = item.VAT;
                    obj.PurchaseOrderDate = item.PurchaseOrderDate;
                    obj.ExpectedDeliveryDate = item.ExpectedDeliveryDate;
                    obj.ShippingAddress = item.ShippingAddress;
                    obj.PaymentTerm = item.PaymentTerm;
                    obj.OtherTermsAndConditions = item.OtherTermsAndConditions;
                    obj.LPOStatusID = item.LPOStatusID;
                    obj.NetAmt = (obj.TotAmt - (obj.VAT / 100));
                    obj.ProcurementTypeId = item.ProcurementTypeId;

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

        public int SaveLPO(LPO item)
        {
            try
            {
                config.LPOes.Add(item);
                config.SaveChanges();
                return item.LPOID;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return 0;
            }
        }

        public LocalPurchaseOrderLookup GetLocalPurchaseOrder(int? param)
        {
            LocalPurchaseOrderLookup purchase_order = new LocalPurchaseOrderLookup();

            try
            {
                var obj = config.LPOes.Where(x => x.LPOID == param).FirstOrDefault();
                if (obj != null)
                {
                    purchase_order.Id = obj.LPOID;
                    purchase_order.LPONumber = obj.LPONo;
                    purchase_order.requisitionNumber = obj.RequisitionNo;
                    purchase_order.nameOfVendor = new Utility { }.GetVendor(obj.VendorID).VendorName;
                    purchase_order.statusOfLPO = new Utility() { }.GetRequisitionStatus(obj.LPOStatusID).RequisitionStatusDesc;
                    purchase_order.LPOTotalAmount = (decimal)obj.TotAmt;
                }

                return purchase_order;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return purchase_order;
            }
        }

        public List<LocalPurchaseOrderLookup> GetLocalPurchaseOrders()
        {
            //get all local purchase orders
            List<LocalPurchaseOrderLookup> purchase_orders = new List<LocalPurchaseOrderLookup>();

            try
            {
                var dta = config.LPOes.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var obj = new LocalPurchaseOrderLookup() 
                        { 
                            Id = d.LPOID,
                            requisitionNumber = d.RequisitionNo,
                            nameOfVendor = new Utility() { }.GetVendor(d.VendorID).VendorName,
                            statusOfLPO = new Utility() { }.GetRequisitionStatus(d.LPOStatusID).RequisitionStatusDesc,
                            LPOTotalAmount = (decimal) d.TotAmt,
                            LPONumber = d.LPONo
                        };

                        purchase_orders.Add(obj);
                    }
                }

                return purchase_orders;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return purchase_orders;
            }
        }

        public List<LocalPurchaseOrderLookup> GetLocalPurchaseOrders(int? _procurementTypeID)
        {
            //get all local purchase orders using the procurementTypeID
            List<LocalPurchaseOrderLookup> purchase_orders = new List<LocalPurchaseOrderLookup>();

            try
            {
                var dta = config.LPOes.Where(x => x.ProcurementTypeId == _procurementTypeID).ToList();
                if (dta.Count() > 0)
                {
                    foreach (var d in dta)
                    {
                        var obj = new LocalPurchaseOrderLookup()
                        {
                            Id = d.LPOID,
                            requisitionNumber = d.RequisitionNo,
                            nameOfVendor = new Utility() { }.GetVendor(d.VendorID).VendorName,
                            statusOfLPO = new Utility() { }.GetRequisitionStatus(d.LPOStatusID).RequisitionStatusDesc,
                            LPOTotalAmount = (decimal)d.TotAmt,
                            LPONumber = d.LPONo,

                            PurchaseDate = d.PurchaseOrderDate,
                            ExpectedDate = d.ExpectedDeliveryDate,
                            ShippingAddress = d.ShippingAddress,
                            PaymentTerm = d.PaymentTerm
                        };

                        purchase_orders.Add(obj);
                    }
                }

                return purchase_orders;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return purchase_orders;
            }
        }

        public List<LocalPurchaseOrderLookup> GetDistinctLPORequisitionNumbers()
        {
            List<LocalPurchaseOrderLookup> retString = new List<LocalPurchaseOrderLookup>();
            try
            {
                var _values = config.LPOes.Select(m => m.LPONo).Distinct().ToList();
                //var _values = config.LPOes.ToList();
                if (_values.Count() > 0)
                {
                    foreach(var v in _values)
                    {
                        var o = new LocalPurchaseOrderLookup() { requisitionNumber = v };
                        retString.Add(o);
                    }
                }

                return retString;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return retString;
            }
        }

        public string GetLPOCount(string LPORef)
        {
            try
            {
                var n = config.LPOes.Select(m => m.LPONo == LPORef).Count();
                var retString = formatNumber(n + 1);
                return string.Format("{0}-{1}", LPORef, retString);
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                return string.Empty;
            }
        }

        private string formatNumber(int n)
        {
            var str = n.ToString();
            if (str.Length == 1)
            {
                return string.Format("{0}{1}", @"000", n.ToString());
            }else if (str.Length == 2)
            {
                return string.Format("{0}{1}", @"00", n.ToString());
            }
            else if(str.Length == 3)
            {
                return string.Format("{0}{1}", @"0", n.ToString());
            }
            else
            {
                return string.Format("{0}", n.ToString());
            }
        }

        #endregion

        #region Capex

        public bool ClearCapex(int _id)
        {
            //clears all current records of capex for the current financial year
            try
            {
                var dta = config.Capexes.Where(c => c.DId == _id).ToList();
                foreach(var d in dta)
                {
                    config.Capexes.Remove(d);
                    config.SaveChanges();
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public bool SaveCapex(Capex item)
        {
            try
            {
                if (item.CapexID == 0)
                {
                    config.Capexes.Add(item);
                    config.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        public List<CapexLookup> GetDepartmentCapexRecords(int dId)
        {
            //gets capex records for selected department
            //modified to fetch department too

            List<CapexLookup> records = new List<CapexLookup>();
            List<Capex> data = null;

            try
            {
                if (dId == 0) 
                {
                    data = config.Capexes.ToList();
                } else 
                {
                    data = config.Capexes.Where(c => c.DId == dId).ToList();
                }
                
                if (data.Count() > 0)
                {
                    foreach(var d in data)
                    {
                        var o = new CapexLookup() { 
                            Id = d.CapexID,
                            itemName = new Utility() { }.GetItem((int) d.CapexItemID).ProductName,
                            deadline = d.EstimatedDeadline,
                            QtyRequested = (int) d.QuantityRequested,
                            QtySupplied = (int) d.QuantitySupplied,
                            QtyOutstanding = (int) d.QuantityOutstanding,
                            justification = d.Justification,
                            capexCategory = new Utility() { }.getItemCategory((int) d.CapexTypeID).nameOfCategory,
                            financialYear = new Utility() { }.getActiveFinancialYear().FinancialYr,

                            nameOfDepartment = new Utility() { }.getDepartment(d.DId).Name
                        };
                        records.Add(o);
                    }
                }

                return records;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return records;
            }
        }

        public bool GetStatusOfCAPEX()
        {
            //gets status of branch CAPEX
            bool bln = false;
            try
            {
                var obj = config.Cfgs.FirstOrDefault(); 
                if (obj != null)
                {
                    bln = obj.C_capex_flag == 1 ? true : false;
                }

                return bln;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return bln;
            }
        }

        public bool SetStatusOfCAPEX(int? _nFlag)
        {
            //sets status of branch CAPEX
            bool bln = false;

            try
            {
                var obj = config.Cfgs.FirstOrDefault();
                if (obj != null)
                {
                    obj.C_capex_flag = _nFlag;
                    config.SaveChanges();
                    bln = true;
                }

                return bln;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return bln;
            }
        }

        #endregion

        #region LPO-Approval

        public List<LPOApprovalLookup> GetLPOApprovalLookups()
        {
            List<LPOApprovalLookup> results = new List<LPOApprovalLookup>();

            try
            {
                var dta = config.LPOApprovals.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var o = new LPOApprovalLookup() {
                            Id = d.ApprovalID,
                            LPO = new RequisitionHelper() { }.GetLocalPurchaseOrder(d.LPO_ID).LPONumber,
                            PersonTag = d.PersonTag,
                            ApprovalDate = Convert.ToDateTime(d.ApprovalDate).ToString("dd-mm-yyyy"),
                            ApprovalStatus = d.ApprovalStatus == 1? "Approved": "Rejected",
                            ApprovalComments = d.ApprovalComments
                        };
                    }
                }

                return results;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return results;
            }
        }

        public List<LPOApprovalLookup> GetLPOApprovalLookups(int _LPOID)
        {
            List<LPOApprovalLookup> results = new List<LPOApprovalLookup>();

            try
            {
                var dta = config.LPOApprovals.Where(x => x.LPO_ID == _LPOID).ToList();
                if (dta.Count() > 0)
                {
                    foreach (var d in dta)
                    {
                        var o = new LPOApprovalLookup()
                        {
                            Id = d.ApprovalID,
                            LPO = new RequisitionHelper() { }.GetLocalPurchaseOrder(d.LPO_ID).LPONumber,
                            PersonTag = d.PersonTag,
                            ApprovalDate = Convert.ToDateTime(d.ApprovalDate).ToString("dd-mm-yyyy"),
                            ApprovalStatus = d.ApprovalStatus == 1 ? "Approved" : "Rejected",
                            ApprovalComments = d.ApprovalComments
                        };

                        results.Add(o);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return results;
            }
        }

        public bool SaveLPOApproval(LPOApproval item)
        {
            try
            {
                config.LPOApprovals.Add(item);
                config.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }

        #endregion

    }

    public struct LPOApprovalLookup
    {
        public int Id { get; set; }
        public string LPO { get; set; }
        public string PersonTag { get; set; }
        public string ApprovalDate { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalComments { get; set; }
    }

    public struct RequisitionLookup
    {
        public int Id { get; set; }
        public string RequisitionNo { get; set; }
        public string Requestee { get; set; }
        public string nameOfDepartment { get; set; }
        public string priority { get; set; }

        public List<reqItem> rLookups { get; set; }

    }

    public struct RequisitionItemLookup
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public Product item { get; set; }
        public int Quantity { get; set; }
        public string narration { get; set; }

        public string status { get; set; }
        public decimal? amt { get; set; }
        public int? LPo_id { get; set; }
    }

    public struct CapexLookup
    {
        public int Id { get; set; }
        public string itemName { get; set; }
        public string capexCategory { get; set; }  //same as item category
        public string deadline { get; set; }
        public int QtyRequested { get; set; }
        public int QtySupplied { get; set; }
        public int QtyOutstanding { get; set; }
        public string justification { get; set; }
        public string financialYear { get; set; }

        public string nameOfDepartment { get; set; }
    }

    public struct LocalPurchaseOrderLookup
    {
        public int Id { get; set; }
        public string requisitionNumber { get; set; }
        public string nameOfVendor { get; set; }
        public string statusOfLPO { get; set; }
        public decimal LPOTotalAmount { get; set; }
        public string LPONumber { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentTerm { get; set; }
        
    }

    public struct reqItem
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string narration { get; set; }

    }

    public struct ConfigLookup
    {
        public int Id { get; set; }
        public int capexFlag { get;set; }
    }

}