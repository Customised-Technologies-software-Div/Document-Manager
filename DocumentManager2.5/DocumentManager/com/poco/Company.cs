using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.com.poco
{
    public class Company
    {
        public int companyId { set; get; }
        public string companyName { set; get; }
        public string companyNameToShow { set; get; }
        public Address address { set; get; }
    }
}
