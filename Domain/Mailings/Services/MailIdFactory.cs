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

        public MailIdFactory(int customerBarcodeId, int sequenceNumber, int dayOfTheYear)
        {
            _barcodeGenerator = new BarcodeGenerator(customerBarcodeId, sequenceNumber, dayOfTheYear);
        }

        public MailIdItem CreateFromAddress(AddressLine addressLine, string language, string priority)
        {
            var item = new MailIdItem
            {
                Sequence = addressLine.Index,
                MailIdNum = _barcodeGenerator.GenerateNext(),
                Language = language,
                Priority = priority,
                AddressDetails = addressLine.AddressDetails
            };

            // Map address components according to bpost specs
            item.Components.Add(new AddressComponent
            {
                Code = 92, // Street
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

            if (!string.IsNullOrEmpty(addressLine.AddressDetails.Contact))
            {
                item.Components.Add(new AddressComponent
                {
                    Code = 90,
                    Value = addressLine.AddressDetails.Contact,
                });
            }

            if (!string.IsNullOrEmpty(addressLine.AddressDetails.Company))
            {
                item.Components.Add(new AddressComponent
                {
                    Code = 91,
                    Value = addressLine.AddressDetails.Company,
                });
            }


            return item;
        }
    }
}
