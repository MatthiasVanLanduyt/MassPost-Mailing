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
        public string PreSortingCode { get; set; } = string.Empty;

        // New properties
        public string Severity
        {
            get
            {
                if (Messages.Any(m => m.Severity == "ERROR"))
                {
                    return "ERROR";
                }
                else if (Messages.Any(m => m.Severity == "WARN"))
                {
                    return "WARN";
                }
                else return "OK";

            }
        }
        public string StatusCode => string.Join(";", Messages
            .Select(m => m.StatusCode)
            .Distinct());
        public string Message => string.Join(";", Messages.Select(m => m.Description).Distinct());

        public bool HasWarnings => Messages.Any(m => m.Severity == "WARN");
        public bool HasErrors => Messages.Any(m => m.Severity == "ERROR");

        public List<AddressResponseMessage> Messages { get; set; } = [];

    }

}
