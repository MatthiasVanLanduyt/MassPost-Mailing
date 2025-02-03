using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings;

namespace Validator.Application.Mailings.Models
{
    internal class MailIdAddressLineMap: ClassMap<MailIdAddressLine>
    {
        public MailIdAddressLineMap()
        {
            Map(m => m.MailIdNum).Index(0);
            Map(m => m.Address.Street).Index(1);
            Map(m => m.Address.PostalCode).Index(2);
            Map(m => m.Address.City).Index(3);
            Map(m => m.Address.Contact).Index(4);

        }
    }
}
