using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Addresses
{
    public class AddressFile
    {
        public List<AddressLine> Addresses { get; set; }
        public AddressFile()
        {
            Addresses = [];
        }
        public void AddAddress(AddressLine address)
        {
            Addresses.Add(address);
        }
        
    }
}
