using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Addresses
{
    public record AddressDetails
    {
        public required string Contact { get; init; }
        public required string Street { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }

        public required string Company { get; init; }

        public override string ToString()
        {
            return $"{Street},{PostalCode} {City} ";
        }
    }
}
