using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using CsvHelper.Configuration.Attributes;
using Sharedkernel;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Services
{
    // Generator for creating the MAIL ID file
    public class TxtMailIdFileGenerator : IMailIdFileGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TxtMailIdFileGenerator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public MailIdFile GenerateFile(MailIdRequest request)
        { 
            using var stream = new MemoryStream();
            var encoding = Encoding.UTF8;
            var writer = new StreamWriter(stream, encoding);

            WriteContent(writer, request);
            writer.Flush();

            string filename = GenerateFileName(request.Header, MailListFileOutputs.TXT);

            return new MailIdFile(
                stream.GetBuffer(),
                filename,
                "text/plain"
            );

        }
        private static void WriteContent(StreamWriter writer, MailIdRequest request)
        {
            var lines = new List<string>
            {
                // Header section
                $"Context|MailingRequest|{MailIdProtocolMetadata.Dataset}|{request.Header.SenderId}|{MailIdProtocolMetadata.Receiver}|{MailIdProtocolMetadata.RequestVersion}",
                $"Header|{request.Header.SenderId}|{request.Header.AccountId}|{request.Options.Mode}",
                $"RequestProps|{request.Header.CustomerFileRef}",
                $"CustomerRef|User|Wim Dutrie|website|www.mailingman.be", // Allows to add own references for file/mailing. Customer ignores this
                $"MailingCreate|1|{request.Header.MailingRef}|{request.Header.DepositIdentifier}|{request.Header.DepositType}|{request.Options.GenMid}|{request.Options.GenPsc}|{request.Header.ExpectedDeliveryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
                $"FileInfo|{MailIdProtocolMetadata.FileInfoCode}",
                $"Format|{request.MailFormat}|"
            };

            
            lines.AddRange(GenerateContactsContent(request.Contacts));
            lines.AddRange(GenerateItemsContent(request));

            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        private static List<string> GenerateItemsContent(MailIdRequest request)
        {
            var lines = new List<string>();
            
            foreach (var item in request.Items)
            {
                lines.Add($"Item|{item.Sequence}|{item.Language}|{item.MailIdNum}||{item.Priority}");

                foreach (var comp in item.Components)
                {
                    lines.Add($"Comp|{comp.Code}|{comp.Value}");
                }
            }

            lines.Add($"ItemCount|{request.ItemCount}");

            return lines;
        }

        private static List<string> GenerateContactsContent(List<Contact> contacts)
        {
            var lines = new List<string>();
            int index = 1;
            foreach (var contact in contacts)
            {
                lines.Add($"Contact|{index}|{contact.FirstName}|{contact.LastName}|{contact.Email}|{contact.LanguageCode}|{contact.Phone}|{contact.Mobile}");
                index++;
            }
            return lines;
        }
        
        private string GenerateFileName(MailIdRequestHeader header, string fileformat)
        {
            return $"{MailIdProtocolMetadata.Receiver}_{MailIdProtocolMetadata.RequestVersion}_{header.SenderId}_{header.CustomerFileRef}_{_dateTimeProvider.TimeStamp}_{MailIdProtocolMetadata.CommunicationStep}.{fileformat}";
        }
    }
}
