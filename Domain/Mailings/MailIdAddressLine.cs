using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses;

namespace Validator.Domain.Mailings
{
    public record MailIdAddressLine
    {
        public required AddressDetails Address { get; init; }
        public required string MailIdNum { get; init; }
    }
}
