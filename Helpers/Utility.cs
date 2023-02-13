using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigiProc;

using System.Diagnostics;


namespace DigiProc.Helpers
{
    public class Utility
    {
        #region Properties

        private ProcurementDbEntities config = new ProcurementDbEntities();

        public VendorType oVendorType { get; set; }
        public ItemCategory itemCategory { get; set; }
        public Item item { get; set; }

        #endregion

        public Utility()
        {

        }

        #region User-Modules
        public List<userModule> getUserModules(string nameOfuser)
        {
            /* method uses the argument to load the user modules to expose on the system */
            var exposed_modules = config.UserModules.Where(x => x.UserName == nameOfuser).ToList();
            List<userModule> list = new List<userModule>();

            foreach(var item in exposed_modules)
            {
                list.Add(new userModule { Id = item.UserModuleID, SystemName = item.Module.SystemName });
            }

            return list;
        }

        #endregion

        #region Item-Category
        public List<ItemCategorization> getItemCategories()
        {
            var item_categories = new List<ItemCategory>();
            var category_list = new List<ItemCategorization>();

            try
            {
                item_categories = config.ItemCategories.ToList();
                if (item_categories.Count() > 0)
                {
                    foreach(var item in item_categories)
                    {
                        ItemCategorization obj = new ItemCategorization() { 
                            Id = item.CategoryID,
                            nameOfCategory = item.CategoryName,
                            descriptionOfCategory = item.CategoryDescription
                        };

                        category_list.Add(obj);
                    }
                }

                return category_list;
            }
            catch(Exception ex)
            {
                Debug.Print($"error: {ex.Message}");
                return category_list;
            }
        }

        public bool SaveItemCategory()
        {
            try
            {
                config.ItemCategories.Add(itemCategory);
                config.SaveChanges();

                return true;
            }
            catch(Exception x)
            {
                throw x;
            }
        }

        public bool UpdateItemCategory()
        {
            try
            {
                var o = config.ItemCategories.Where(c => c.CategoryID == itemCategory.CategoryID).FirstOrDefault();
                o.CategoryName = itemCategory.CategoryName;
                o.CategoryDescription = itemCategory.CategoryDescription;

                config.SaveChanges();
                return true;
            }
            catch(Exception x)
            {
                throw x;
            }
        }

        public ItemCategorization getItemCategory(int _categoryID)
        {
            //gets item categorization using id
            ItemCategorization obj = new ItemCategorization();

            try
            {
                var o = config.ItemCategories.Where(ic => ic.CategoryID == _categoryID).FirstOrDefault();
                if (o != null)
                {
                    obj.Id = o.CategoryID;
                    obj.nameOfCategory = o.CategoryName;
                    obj.descriptionOfCategory = o.CategoryDescription;
                }

                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region StandardUnit
        public List<StandardUnit> getItemMetrics()
        {
            var item_metrics = new List<SIUnit>();
            var formatted_metrics = new List<StandardUnit>();

            try
            {
                item_metrics = config.SIUnits.ToList();
                if (item_metrics.Count() > 0)
                {
                    foreach(var item in item_metrics)
                    {
                        StandardUnit obj = new StandardUnit() { 
                            Id = item.SIUnitID,
                            Metric = item.Metric,
                            Measurement = item.Measurement
                        };

                        formatted_metrics.Add(obj);
                    }
                }

                return formatted_metrics;
            }
            catch(Exception ex)
            {
                return formatted_metrics;
            }
        }

        #endregion

        #region BusinessType
        public List<BusinessType> getBusinessTypes()
        {
            var business_types = new List<BusinessType>();
            try
            {
                var data = config.BusinessTypes.ToList();
                if (data.Count() > 0)
                {
                    foreach(var d in data)
                    {
                        var b = new BusinessType() { 
                             BusinessTypeID = d.BusinessTypeID,
                             BusinessDescription = d.BusinessDescription
                        };

                        business_types.Add(b);
                    }
                }

                return business_types;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return business_types;
            }
        }

        #endregion

        #region Accounting Periods

        public List<FinancialYear> getFinancialYearsAsync()
        {
            //gets financial years
            var account_periods = new List<FinancialYear>();
            try
            {
                var data = config.FinancialYears.Where(x => x.IsActive == 1).ToList();
                if (data.Count() > 0)
                {
                    foreach(var d in data)
                    {
                        var o = new FinancialYear() { 
                            FinancialYrID = d.FinancialYrID,
                            FinancialYr = d.FinancialYr,
                            Description = d.Description,
                            IsActive = d.IsActive
                        };

                        account_periods.Add(o);
                    }
                }

                return account_periods;
            }
            catch (Exception x)
            {
                Debug.Print(x.Message);
                return account_periods;
            }
        }

        public FinancialYear getActiveFinancialYear()
        {
            FinancialYear obj = null;
            try
            {
                obj = config.FinancialYears.Where(x => x.IsActive == 1).FirstOrDefault();
                return obj;
            }
            catch (Exception x)
            {
                Debug.Print(x.Message);
                return obj;
            }
        }

        public bool saveFinancialYear(int periodFrm, int periodTo, string brief)
        {
            try
            {
                var obj = new FinancialYear() { FinancialYr = $"{periodFrm}-{periodTo}", Description = brief };
                config.FinancialYears.Add(obj);
                config.SaveChanges();
                return true;
            }
            catch(Exception x)
            {
                Debug.Print($"error: {x.Message}");
                return false;
            }
        }

        #endregion

        #region Currency

        public bool saveCurrency(string cName, string cSymbol)
        {
            try
            {
                var currencyObj = new Currency() { CurrencyName = cName, CurrencySymbol = cSymbol };
                config.Currencies.Add(currencyObj);
                config.SaveChanges();

                return true;
            }
            catch(Exception x)
            {
                Debug.Print($"error: {x.Message}");
                return false;
            }
        }

        public bool updateCurrency(int id, string cName, string cSymbol)
        {
            try
            {
                var obj = config.Currencies.Where(c => c.CurrencyID == id).FirstOrDefault();

                obj.CurrencyName = cName;
                obj.CurrencySymbol = cSymbol;
                config.SaveChanges();

                return true;
            }
            catch(Exception x)
            {
                Debug.Print($"error: {x.Message}");
                return false;
            }
        }

        public List<Denomination> getCurrencies()
        {
            List<Currency> currency_list = new List<Currency>();
            List<Denomination> denomList = new List<Denomination>();

            try
            {
                currency_list = config.Currencies.ToList();
                if (currency_list.Count() > 0)
                {
                    foreach(var item in currency_list)
                    {
                        var obj = new Denomination() { 
                            Id = item.CurrencyID,
                            nameOfcurrency = item.CurrencyName,
                            denominationSymbol = item.CurrencySymbol
                        };

                        denomList.Add(obj);
                    }
                }

                return denomList.ToList();
            }
            catch(Exception x)
            {
                return denomList;
            }
        }

        #endregion

        #region Vendor-Type

        public bool saveVendorType()
        {
            try
            {
                config.VendorTypes.Add(oVendorType);
                config.SaveChanges();

                return true;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
        }

        public bool updateVendorType()
        {
            try
            {
                var obj = config.VendorTypes.Where(x => x.VendorTypeID == oVendorType.VendorTypeID).FirstOrDefault();
                obj.VendorDescription = oVendorType.VendorDescription;
                config.SaveChanges();

                return true;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return false;
            }
        }

        public List<VendorTypeLookup> getVendorTypes()
        {
            List<VendorType> vendorTypes = new List<VendorType>();
            List<VendorTypeLookup> vendor_types = new List<VendorTypeLookup>();

            try
            {
                vendorTypes =  config.VendorTypes.ToList();
                if (vendorTypes.Count() > 0)
                {
                    foreach(var item in vendorTypes)
                    {
                        var o = new VendorTypeLookup()
                        {
                            Id = item.VendorTypeID,
                            Description = item.VendorDescription
                        };

                        vendor_types.Add(o);
                    }
                }

                return vendor_types;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Item

        public bool SaveItem(string measuredIn)
        {
            //saves item to the data store
            try
            {
                int unitId = new StandardUnit() { }.get(config, measuredIn).Id;
                var obj = new SIUnit { SIUnitID = unitId };
                item.SIUnitID = obj.SIUnitID;
                config.Items.Add(item); 
                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateItem()
        {
            try
            {
                var o = config.Items.Where(i => i.ItemID == item.ItemID).FirstOrDefault();

                o.ItemName = item.ItemName;
                o.ItemCode = item.ItemCode;
                o.ItemCategoryID = item.ItemCategoryID;
                o.MinStockLevel = item.MinStockLevel;
                o.MaxStockLevel = item.MaxStockLevel;
                o.ItemDescription = item.ItemDescription;

                config.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Product GetItem(int itemID)
        {
            //gets product using itemID
            Product product = new Product();
            try
            {
                var item = config.Items.Where(i => i.ItemID == itemID).FirstOrDefault();
                if (item != null)
                {

                    product.Id = item.ItemID;
                    product.ProductName = item.ItemName;
                    product.ProductCode = item.ItemCode;
                    product.ProductMinimumStock = (int)item.MinStockLevel;
                    product.ProductMaximumStock = (int)item.MaxStockLevel;
                    product.ProductDescription = item.ItemDescription;
                    product.SIUnit = new StandardUnit { }.get(config, (int)item.SIUnitID);
                    product.ProductCategoryId = (int)item.ItemCategoryID;
                }

                return product;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return product;
            }
        }

        public List<Product> GetItems()
        {
            //gets all the items in the data store
            var item_list = new List<Item>();
            var product_list = new List<Product>();

            try
            {
                item_list = config.Items.ToList();
                if (item_list.Count() > 0)
                {
                    foreach(var item in item_list)
                    {
                        var obj = new Product() { 
                            Id = item.ItemID,
                            ProductName = item.ItemName,
                            ProductCode = item.ItemCode,
                            ProductMinimumStock = (int)item.MinStockLevel,
                            ProductMaximumStock = (int)item.MaxStockLevel,
                            ProductDescription = item.ItemDescription,
                            SIUnit = new StandardUnit { }.get(config, (int)item.SIUnitID)
                        };

                        product_list.Add(obj);
                    }
                }

                return product_list;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string generateItemCode(string itemcategory, int itemcategoryID)
        {
            //method is responsible for the generation of item code
            try
            {
                var prefix = generatePrefix(itemcategory);
                var nextID = getNextCounterID(itemcategoryID);
                var formattedID = formatNextID(nextID.ToString());

                return String.Format("{0}-{1}", prefix, formattedID);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private string generatePrefix(string strItem_category)
        {
            var str = strItem_category.Substring(0, 2);
            return string.Format("{0}", str.ToUpper());
        }

        private int getNextCounterID(int id)
        {
            //gets the highest number..add one to it and return
            try
            {
                int currentMax = config.Items.Where(m => m.ItemCategoryID == id).Max(x => x.ItemID);
                return currentMax += 1;
            }
            catch(Exception x)
            {
                return 1;
            }
            
        }

        private string formatNextID(string nextID)
        {
            int LEN = nextID.Length;
            if (LEN == 1)
            {
                nextID = string.Format("{0}{1}", @"000", nextID);
            }
            else if(LEN == 2)
            {
                nextID = string.Format("{0}{1}", @"00", nextID);
            }
            else if(LEN == 3)
            {
                nextID = string.Format("{0}{1}", @"0", nextID);
            }

            return nextID;
        }

        public int getItemID(string icode)
        {
            //get itemId using item code
            try
            {
                var obj = config.Items.Where(i => i.ItemCode == icode).FirstOrDefault();
                return obj.ItemID;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return 0;
            }
        }

        #endregion

        #region Priority

        public List<Priority> GetPriorityTypes()
        {
            List<PriorityType> priorityTypes = new List<PriorityType>();
            List<Priority> data = new List<Priority>();

            try
            {
                priorityTypes = config.PriorityTypes.ToList();
                if (priorityTypes.Count() > 0)
                {
                    foreach(var item in priorityTypes)
                    {
                        var p = new Priority() { Id = item.PriorityID, nameOfPriority = item.PriorityDescription };
                        data.Add(p);
                    }
                }

                return data.ToList();
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                throw x;
            }
        }

        public PriorityType GetPriority(int? _id)
        {
            //gets priority
            PriorityType obj = null;
            try
            {
                obj = config.PriorityTypes.Where(p => p.PriorityID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region Requisition-Types

        public List<RequisitionType> GetRequisitionTypes()
        {
            List<RequisitionType> requisition_types = new List<RequisitionType>();
            try
            {
                var _data = config.RequisitionTypes.ToList();
                if (_data.Count() > 0)
                {
                    foreach(var d in _data)
                    {
                        var rq = new RequisitionType() 
                        { 
                            RequisitionTypeID = d.RequisitionTypeID,
                            RequisitionType1 = d.RequisitionType1,
                            RequisitionDescription = d.RequisitionDescription
                        };

                        requisition_types.Add(rq);
                    }
                }

                return requisition_types;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return requisition_types;
            }
        }


        #endregion

        #region Company

        public Company getDefaultCompany()
        {
            Company obj = null;

            try
            {
                var o = config.Companies.Where(c => c.CompanyID == 1).FirstOrDefault();
                if (o.CompanyID > 0)
                {
                    obj = new Company() { 
                        CompanyID = o.CompanyID,
                        CompanyDescription = o.CompanyDescription
                    };
                }

                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        #endregion

        #region Departments

        public Department getDepartment(string str)
        {
            Department obj = null;

            try
            {
                obj = config.Departments.Where(d => d.Name == str).FirstOrDefault();
                return obj;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return obj;
            }
        }

        public Department getDepartment(int? _id)
        {
            Department obj = null;

            try
            {
                obj = config.Departments.Where(d => d.DepartmentID == _id).FirstOrDefault();
                return obj;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return obj;
            }
        }

        public List<Department> GetDepartments()
        {
            List<Department> departments = null;
            try
            {
                var list = config.Departments.ToList();
                if (list.Count() > 0)
                {
                    departments = new List<Department>();

                    foreach(var item in list)
                    {
                        var d = new Department() { 
                            DepartmentID = item.DepartmentID,
                            Name = item.Name,
                            Head = item.Head
                        };

                        departments.Add(d);
                    }
                }

                return departments;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return departments;
            }
        }

        #endregion

        #region RequisitionStatus

        public RequisitionStatu GetRequisitionStatus(string str)
        {
            RequisitionStatu obj = null;
            try
            {
                obj = config.RequisitionStatus.Where(r => r.RequisitionStatusDesc == str).FirstOrDefault();
                return obj;
            }
            catch(Exception x)
            {
                Debug.Print(x.Message);
                return obj;
            }
        }

        public RequisitionStatu GetRequisitionStatus(int? id)
        {
            RequisitionStatu obj = null;
            try
            {
                obj = config.RequisitionStatus.Where(r => r.RequisitionStatusID == id).FirstOrDefault();
                return obj;
            }
            catch (Exception x)
            {
                Debug.Print(x.Message);
                return obj;
            }
        }

        #endregion

        #region Vendor-List

        public List<Vendor> GetVendors()
        {
            //method gets all vendors in the system
            List<Vendor> vendor_list = new List<Vendor>();
            try
            {
                var dta = config.Vendors.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var vendor = new Vendor() { 
                            VendorID = d.VendorID,
                            VendorNo = d.VendorNo,
                            VendorName = d.VendorName,
                            VendorLocation = d.VendorLocation,
                            ContactPerson = d.ContactPerson,
                            NameOfOwner = d.NameOfOwner,
                            CompanyContact = d.CompanyContact,
                            CompanyHomeContact = d.CompanyHomeContact
                        };

                        vendor_list.Add(vendor);
                    }
                }
                else { return vendor_list; }

                return vendor_list;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return vendor_list;
            }
        }

        public Vendor GetVendor(int? vendorID)
        {
            //gets a vendor using vendor id
            Vendor obj = null;

            try
            {
                obj = config.Vendors.Where(v => v.VendorID == vendorID).FirstOrDefault();
                return obj;
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                return obj;
            }
        }

        #endregion

        #region Procurement-Type

        public List<ProcurementType> GetProcurementTypes()
        {
            //method gets procurement types from the data store
            List<ProcurementType> procurement_types = new List<ProcurementType>();
            try
            {
                var dta = config.ProcurementTypes.ToList();
                if (dta.Count() > 0)
                {
                    foreach(var d in dta)
                    {
                        var p = new ProcurementType() { 
                            ProcurementTypeID = d.ProcurementTypeID,
                            ProcurementDescription = d.ProcurementDescription
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

        #endregion

    }

    #region Structs
    public struct userModule
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
    }

    public struct StandardUnit
    {
        public int Id { get; set; }
        public string Metric { get; set; }
        public string Measurement { get; set; }

        public StandardUnit get(ProcurementDbEntities config, int _Id)
        {
            //gets the standard unit using id
            StandardUnit su = new StandardUnit();
            try
            {
                var o = config.SIUnits.Where(x => x.SIUnitID == _Id).FirstOrDefault();
                if (o != null)
                {
                    su.Id = o.SIUnitID;
                    su.Metric = o.Metric;
                    su.Measurement = o.Measurement;
                }

                return su;
            }
            catch(Exception x)
            {
                throw x;
            }
        }

        public StandardUnit get(ProcurementDbEntities config, string strMeasuredIn)
        {
            //gets the standard unit using id
            StandardUnit su = new StandardUnit();
            try
            {
                var o = config.SIUnits.Where(x => x.Measurement == strMeasuredIn).FirstOrDefault();
                if (o != null)
                {
                    su.Id = o.SIUnitID;
                    su.Metric = o.Metric;
                    su.Measurement = o.Measurement;
                }

                return su;
            }
            catch (Exception x)
            {
                throw x;
            }
        }

    }
    
    public struct ItemCategorization
    {
        public int Id { get; set; }
        public string nameOfCategory { get; set; }
        public string descriptionOfCategory { get; set; }
    }

    public struct Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int ProductMinimumStock { get; set; }
        public int ProductMaximumStock { get; set; }
        public string ProductDescription { get; set; }
        public StandardUnit SIUnit { get; set; }

        public int ProductCategoryId { get; set; }
    }

    public struct VendorTypeLookup
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public struct Denomination
    {
        public int Id { get; set; }
        public string nameOfcurrency { get; set; }
        public string denominationSymbol { get; set; }
    }

    public struct Priority
    {
        public int Id { get; set; }
        public string nameOfPriority { get; set; }
    }

    public struct UserSession
    {
        public string userName { get; set; }
        public Department userDepartment { get; set; }

        public string userProfile { get; set;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               }
        public string moduleString { get; set; }
        public List<userModule> modules { get; set; }

        public string approverTag { get; set; }
        public string bioName { get; set; }
    }

    #endregion

}