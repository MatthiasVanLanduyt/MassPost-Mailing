using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Models
{
    internal class MailIdItemMap: ClassMap<MailIdItem>
    {
        public MailIdItemMap()
        {
            Map(m => m.EanBarCode).Index(0);
            Map(m => m.AddressDetails.Street).Index(1);
            Map(m => m.AddressDetails.PostalCode).Index(2);
            Map(m => m.AddressDetails.City).Index(3);
            Map(m => m.AddressDetails.Contact).Index(4);
            Map(m => m.AddressDetails.Company).Index(5);

        }
    }
}
