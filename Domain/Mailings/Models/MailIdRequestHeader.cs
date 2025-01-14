using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdRequestHeader
    {
        public required int CustomerId { get; init; }
        public required int AccountId { get; init; }
        public required string MailingRef { get; init; }
        public required string ExpectedDeliveryDate { get; init; }
        public required string SerialNumber { get; init; }
        public string Mode { get; init; } = "P"; // P=Production, T=Test, C=Certification
    }
}
