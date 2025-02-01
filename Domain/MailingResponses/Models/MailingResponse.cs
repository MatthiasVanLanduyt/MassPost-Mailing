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
        public decimal AddressComplianceRate { get; set; }
        public decimal BuildingComplianceRate { get; set; }
        public decimal PresortingComplianceRate { get; set; }

        public int TotalRecords => Addresses.Count;

        public int ValidRecords => Addresses.Count(a => !(a.HasWarnings || a.HasErrors));
        public int RecordsWithWarnings => Addresses.Count(a => a.HasWarnings);
        public int RecordsWithErrors => Addresses.Count(a => a.HasErrors);

        public List<AddressResponse> Addresses { get; set; } = [];
    }
}
