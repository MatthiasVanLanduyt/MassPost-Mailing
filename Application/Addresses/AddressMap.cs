using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Validator.Domain.Addresses;

namespace Validator.Application.Addresses
{
    internal class AddressMap: ClassMap<AddressDetails>
    {
        public AddressMap()
        {
            Map(m => m.Street).Index(0);
            Map(m => m.PostalCode).Index(1);
            Map(m => m.City).Index(2);
            Map(m => m.Contact).Index(3);
            Map(m => m.Company).Index(4);
            
        }
    }
    
}
