using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings.Models;

namespace Validator.Domain.Mailings.Services
{
    public class MailIdFactory
    {
        private readonly BarcodeGenerator _barcodeGenerator;

        public MailIdFactory(int customerBarcodeId, int depositId, int dayOfTheYear)
        {
            _barcodeGenerator = new BarcodeGenerator(customerBarcodeId, depositId, dayOfTheYear);
        }

        public MailIdItem CreateFromAddress(AddressLine addressLine)
        {
            var item = new MailIdItem
            {
                Sequence = addressLine.Index,
                MailIdNum = _barcodeGenerator.GenerateNext()
            };

            // Map address components according to bpost specs
            item.Components.Add(new AddressComponent
            {
                Code = 9, // Street
                Value = addressLine.AddressDetails.Street
            });

            item.Components.Add(new AddressComponent
            {
                Code = 15, // Postal code
                Value = addressLine.AddressDetails.PostalCode.ToString()
            });

            item.Components.Add(new AddressComponent
            {
                Code = 16, // City
                Value = addressLine.AddressDetails.City
            });

            // Split contact into first/last name if needed
            if (!string.IsNullOrEmpty(addressLine.AddressDetails.Contact))
            {
                var names = addressLine.AddressDetails.Contact.Split(' ', 2);
                if (names.Length > 0)
                {
                    item.Components.Add(new AddressComponent
                    {
                        Code = 2, // First name
                        Value = names[0]
                    });
                }
                if (names.Length > 1)
                {
                    item.Components.Add(new AddressComponent
                    {
                        Code = 4, // Last name  
                        Value = names[1]
                    });
                }
            }

            return item;
        }
    }
}
