using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Addresses
{
    public interface IPostalCodeService
    {
        Dictionary<string, HashSet<string>> PostalCodeCities { get; }
    }
}
