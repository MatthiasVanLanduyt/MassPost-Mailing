using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdRequestHeader
    {
        public required int SenderId{ get; init; }
        
        public required int AccountId { get; init; } //accountID or e-Masspost account numbe
        public required string MailingRef { get; init; }
        public required DateOnly ExpectedDeliveryDate { get; init; }
        public required int CustomerBarcodeId { get; init; }
        public required string CustomerFileRef { get; init; }

        //TODO: discuss with Mailingman if we could have an identifier for the project
        public string Mode { get; init; } = "T"; // P=Production, T=Test, C=Certification
    }
}
