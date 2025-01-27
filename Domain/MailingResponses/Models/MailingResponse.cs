using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public class MailingResponse
    {
        public string MailingRef { get; set; }
        public string Status { get; set; }
        public string AddressComplianceRate { get; set; }
        public string BuildingComplianceRate { get; set; }
        public string PresortingComplianceRate { get; set; }
        public List<AddressResponse> Addresses { get; set; } = [];
    }
}
