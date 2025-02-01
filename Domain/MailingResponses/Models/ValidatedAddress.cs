using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses;

namespace Validator.Domain.MailingResponses.Models
{
    public class ValidatedAddress
    {
        public AddressDetails AddressDetails { get; set; }
        public string StatusCode { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }

    }
}
