using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.com.poco
{
    public class Address
    {
        public int addressID { set; get; }
        public int companyID { set; get; }
        public string address1 { set; get; }
        public string address2 { set; get; }
        public string address3 { set; get; }
        public string city { set; get; }
        public string state { set; get; }
        public string pincode { set; get; }
        public int stateCode { set; get; }
        public string phone { set; get; }
        public string GSTNo { set; get; }
    }

}
