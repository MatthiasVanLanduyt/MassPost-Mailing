using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public class AddressResponse
    {
        public int Sequence { get; set; }
        public string PreSortingCode { get; set; }
        public string Status { get; set; }

        public bool HasWarnings => Messages.Any(m => m.Severity == "WARN");
        public bool HasErrors => Messages.Any(m => m.Severity == "ERROR");

        public List<AddressResponseMessage> Messages { get; set; } = [];
    }

}
