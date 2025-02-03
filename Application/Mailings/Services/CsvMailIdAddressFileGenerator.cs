using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Sharedkernel;
using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings;

namespace Validator.Application.Mailings.Services
{
    public class CsvMailIdAddressFileGenerator
    {

        public MailIdFile GenerateFile(List<MailIdAddressLine> addresses, string filename)
        {
            var culture = new CultureInfo("nl-BE");
            Console.WriteLine("Reading CSV file");

            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, culture))
            {
                csv.Context.RegisterClassMap<MailIdAddressLineMap>();
                csv.WriteRecords(addresses);
                writer.Flush();
            }
            
            return new MailIdFile(
                memoryStream.ToArray(),
                filename,
                "text/csv"
            );
        }
    }
}
