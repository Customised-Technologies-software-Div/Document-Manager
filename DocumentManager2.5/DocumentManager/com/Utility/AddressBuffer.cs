using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.com.poco;
namespace DocumentManager.com.Utility
{
    public class AddressBuffer
    {
        private int current;
        private int size;
        private List<Address> addresses;
        public AddressBuffer(List<Address> addresses)
        {
            this.addresses = addresses;
            current = 1;
            size = this.addresses.Count;
        }
        public void AddAddress(Address address)
        {
            this.addresses.Add(address);
            size += 1;
        }
        public void RemoveLast()
        {
            this.addresses.RemoveAt(size-1);
            size -= 1;
        }
        public Address GetAddress(int index)
        {
            return addresses.ElementAt(index - 1);
        }
        public Address GetNextAddress()
        {
            current = current + 1;
            return GetAddress(current);
        }
        public Address GetPreviousAddress()
        {
            current = current - 1;
            return GetAddress(current);
        }
        public Address GetCurrentAddress()
        {
            return GetAddress(current);
        }
        public int GetSize()
        {
            return this.size;
        }
        public int GetCurrentIndex()
        {
            return this.current;
        }
        public bool canGetNext()
        {
            return (current < size) ? true : false;
        }
        public bool canGetPrev()
        {
            return (current > 1) ? true : false;
        }
    }
}
