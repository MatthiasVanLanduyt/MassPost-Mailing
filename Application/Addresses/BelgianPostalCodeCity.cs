using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Validator.Application.Addresses
{
    public record BelgianPostalCodeCity
    {
        [JsonPropertyName("Postcode")]
        public required int PostalCode { get; init; }

        [JsonPropertyName("Plaatsnaam")]
        public required string City { get; init; } = string.Empty;
    }
}
