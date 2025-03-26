using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Services
{
    public class TruncateFieldsToMaxLengthService
    {
        public string TruncateAddressComponent(AddressComponent component)
        {
            if (string.IsNullOrEmpty(component.Value))
                return component.Value;
            // Truncate the value if needed
            int maxLength = MaxFieldLengths.GetMaxLength(component.Code);
            string truncatedValue = component.Value.Length > maxLength ?
                component.Value.Substring(0, maxLength) :
                component.Value;

            return truncatedValue;
        }

        public string TruncateField(string field, int maxLength)
        {
            // Truncate the value if needed
            string truncatedValue = field.Length > maxLength ?
                field.Substring(0, maxLength) :
                field;

            return field;
        }
    }
}
