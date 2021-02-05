using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.com.poco
{
    public class Contact
    {
        public int contactId { set; get; }
        public int companyId { set; get; }
        [DisplayName("Name")]
        public string contact_name { set; get; }
        [DisplayName("Email")]
        public string contact_email { set; get; }
        [DisplayName("Phone")]
        public string contact_phone { set; get; }
    }

}
