using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record AddressComponent
    {
        public int Code { get; init; }
        public string Value { get; init; }
    }
}
