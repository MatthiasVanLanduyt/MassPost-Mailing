using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Models;
using Validator.Domain.MailingResponses.Models;

namespace Validator.Application.Mailings.Services
{ 
    public class MailingResponseCsvExporter : IMailingResponseCsvExporter
    {
        public MailingResponseCsvExporter()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public MailResponseFile ExportToCsv(MailingResponse response)
        {
            var filename = $"MailingResponse_{response.MailingRef}_{DateTime.Now:yyyyMMddHHmmss}.csv";

            var culture = new CultureInfo("nl-BE");

            Encoding windows1252Encoding = Encoding.GetEncoding(1252);

            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, windows1252Encoding))
            using (var csv = new CsvWriter(writer, culture))
            {
                csv.Context.RegisterClassMap<MailingResponseExportMap>();

                var exportData = response.AddressResponses.Select(addr => new MailingResponseExportModel
                {
                    Sequence = addr.Sequence,
                    PreSortingCode = addr.PreSortingCode,
                    Severity = addr.Severity,
                    StatusCode = addr.StatusCode,
                    Messages = addr.Message,

                });

                csv.WriteRecords(exportData);
                writer.Flush();
            }

            return new MailResponseFile(
                memoryStream.ToArray(),
                filename,
                "text/csv"
            );
        }
    }
}
