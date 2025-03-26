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
        public required AddressDetails AddressDetails { get; set; }
        public required string StatusCode { get; set; }
        public required string Severity { get; set; }
        public required string Message { get; set; }

    }
}
