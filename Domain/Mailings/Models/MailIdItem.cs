using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdItem
    {
        public int Sequence { get; init; }

        public string EanBarCode => $"JJBEA{MailIdNum}";
        public required string MailIdNum { get; set; } // MAIL ID barcode
        public string Priority { get; init; } = "NP"; // P or NP
        public string Language { get; init; } = "nl"; // fr, nl, de
        public List<AddressComponent> Components { get; set; } = [];

        public AddressDetails AddressDetails { get; set; }
    }
}
