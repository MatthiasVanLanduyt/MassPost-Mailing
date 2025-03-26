using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public class MailingResponse
    {
        public string MailingRef { get; set; } = string.Empty;

        public MailingResponseHeader Header { get; set; } = new MailingResponseHeader();
        public string Status { get; set; } = string.Empty;
        public decimal AddressComplianceRate { get; set; }
        public decimal BuildingComplianceRate { get; set; }
        public decimal PresortingComplianceRate { get; set; }

        public int TotalRecords => AddressResponses.Count;

        public int ValidRecords => AddressResponses.Count(a => !(a.HasWarnings || a.HasErrors));
        public int RecordsWithWarnings => AddressResponses.Count(a => a.HasWarnings);
        public int RecordsWithErrors => AddressResponses.Count(a => a.HasErrors);

        public List<AddressResponse> AddressResponses { get; set; } = [];
    }
}
