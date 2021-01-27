using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.com.poco
{
    class Document
    {
        [DisplayName("S.No.")]
        public int SNo { set; get; }
        [DisplayName("R.No")]
        public int RNo { set; get; }
        [DisplayName("Sender")]
        public string sender { set; get; }
        [DisplayName("Company Name")]
        public string companyName { set; get; }
        [DisplayName("Date")]
        public string date { set; get; }
    }
}
