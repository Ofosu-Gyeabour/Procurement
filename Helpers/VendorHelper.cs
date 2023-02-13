using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigiProc;
using System.Diagnostics;


namespace DigiProc.Helpers
{
    public class VendorHelper
    {
        private ProcurementDbEntities config = new ProcurementDbEntities();

        public Vendor oVendor { get; set; }

        #region Vendor-ID Generation
        public string generateVendorID()
        {
            //generates the vendor ID
            int nextVendorID = this.getMaxVendor();
            string formattedVendorNo = this.formatVendorID(nextVendorID.ToString());

            return string.Format("{0}-{1}", @"VND", formattedVendorNo);
        }

        public int getMaxVendor()
        {
            try
            {
                int vID = config.Vendors.Max(m => m.VendorID);
                return vID + 1;
            }
            catch(Exception x)
            {
                Debug.Print($"error: {x.Message}");
                return 1;
            }
        }

        public string formatVendorID(string nextID)
        {
            int LEN = nextID.Length;
            if (LEN == 1)
            {
                nextID = string.Format("{0}{1}", @"000", nextID);
            }
            else if (LEN == 2)
            {
                nextID = string.Format("{0}{1}", @"00", nextID);
            }
            else if (LEN == 3)
            {
                nextID = string.Format("{0}{1}", @"0", nextID);
            }

            return nextID;
        }

        #endregion
    
        public bool SaveBasicInformation()
        {
            try
            {
                if (oVendor.VendorID == 0)
                {
                    config.Vendors.Add(oVendor);
                    config.SaveChanges();
                    return true;
                }
                else
                {
                    var o = config.Vendors.Where(v => v.VendorID == oVendor.VendorID).FirstOrDefault();

                    o.VendorNo = oVendor.VendorNo;
                    o.VendorName = oVendor.VendorName;
                    o.VendorLocation = oVendor.VendorLocation;
                    o.ContactPerson = oVendor.ContactPerson;
                    o.VendorTypeID = oVendor.VendorTypeID;
                    o.TIN = oVendor.TIN;
                    o.CompanyRegistrationNo = oVendor.CompanyRegistrationNo;
                    o.IncorporationDate = oVendor.IncorporationDate;
                    o.CompanyBusinessTypeID = oVendor.CompanyBusinessTypeID;
                    o.CompanyContact = oVendor.CompanyContact;
                    o.CompanyHomeContact = oVendor.CompanyHomeContact;
                    o.CompanyEmailAddress = oVendor.CompanyEmailAddress;
                    o.CompanyGHPostAddress = oVendor.CompanyGHPostAddress;
                    o.CompanyWebsite = oVendor.CompanyWebsite;
                    o.CompanyLinkedIn = oVendor.CompanyLinkedIn;
                    o.CompanyFb = oVendor.CompanyFb;

                    config.SaveChanges();
                    return true;
                }

            }
            catch(Exception x)
            {
                Debug.Print($"error: {x.Message}");
                throw x;
            }
        }

    }

    public struct CommunicationDetails
    {

    }

    public struct Directorship
    {

    }

    public struct EstimatedTurnOver
    {

    }

}