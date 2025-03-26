using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace Validator.Application.Mailings.Models
{
    public record MailingResponseExportModel
    {
        public required int Sequence { get; set; }
        public required string PreSortingCode { get; set; }
        public required string Severity { get; set; }
        public required string StatusCode { get; set; }  
        public required string Messages { get; set; }
    }

    public class MailingResponseExportMap : ClassMap<MailingResponseExportModel>
    {
        public MailingResponseExportMap()
        {
            Map(m => m.Sequence);
            Map(m => m.PreSortingCode);
            Map(m => m.Severity);
            Map(m => m.StatusCode);
            Map(m => m.Messages);
        }
    }
}
