using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using CsvHelper.Configuration.Attributes;
using Sharedkernel;
using Validator.Application.Mailings.Contracts;
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
            string content = GenerateTxtContent(request);
            string filename = GenerateFileName(request.Header, MailListFileOutputs.TXT);

            return new MailIdFile(content, filename, "text/plain", Encoding.UTF8);

        }
        private string GenerateTxtContent(MailIdRequest request)
        {
            var lines = new List<string>
            {
                // Header section
                $"Context|MailingRequest|{MailIdProtocolMetadata.Dataset}|{request.Header.SenderId}|{MailIdProtocolMetadata.Receiver}|{MailIdProtocolMetadata.RequestVersion}",
                $"Header|{request.Header.SenderId}|{request.Header.AccountId}|{request.Header.Mode}",
                $"RequestProps|{request.Header.MailingRef}",
                $"CustomerRef|User|Wim Dutrie|website|www.mailingman.be", // Allows to add own references for file/mailing. Customer ignores this
                $"MailingCreate|1|{request.Header.MailingRef}|{request.Options.DepositId}|{request.Options.DepositIdentifierType}|{request.Options.GenMid}|{request.Options.GenPSC}|{request.Header.ExpectedDeliveryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
                $"FileInfo|{MailIdProtocolMetadata.FileCode}",
                $"Format|{request.MailFormat}"
            };

            
            lines.AddRange(GenerateContactsContent(request.Contacts));
            lines.AddRange(GenerateItemsContent(request));

            return string.Join(Environment.NewLine, lines);
        }

        private List<string> GenerateItemsContent(MailIdRequest request)
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

        private List<string> GenerateContactsContent(List<Contact> contacts)
        {
            var lines = new List<string>();
            int index = 1;
            foreach (var contact in contacts)
            {
                lines.Add($"Contact|{index}|{contact.FirstName}|{contact.LastName}|{contact.Email}|{contact.LanguageCode}|{contact.Phone}||{contact.Mobile}");
                index++;
            }
            return lines;
        }
        
        private string GenerateFileName(MailIdRequestHeader header, string fileformat)
        {
            return $"{MailIdProtocolMetadata.FileCode}_{MailIdProtocolMetadata.RequestVersion}_{header.SenderId}_{header.MailingRef}_{_dateTimeProvider.TimeStamp}_{MailIdProtocolMetadata.CommunicationStep}_{fileformat}";
        }
    }
}
