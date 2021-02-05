using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.com.poco
{
    /// <summary>
    /// This class is used for storing values from text fields and generating report from that values
    /// </summary>
    public class Field
    {
        public string companyName { set; get; }
        public string address1 { set; get; }
        public string address2 { set; get; }
        public string address3 { set; get; }
        public string city { set; get; }
        public string pincode { set; get; }
        public string phone { set; get; }
        public string state { set; get; }
        public string country { set; get; }
        public string refNo { set; get; }
        public string date { set; get; }
        public string stateCode { set; get; }
        public string contactName { set; get; }
        public string contactEmail { set; get; }
        public string contactPhone { set; get; }
        public string gstNo { set; get; }
        public string serviceInvoiceNo { set; get; }
        public string taxInvoiceNo { set; get; }
    }

}
