using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sharedkernel;
using Validator.Application.Mailings.Contracts;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Services
{
    public class XMLMailIdGenerator : IMailIdFileGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public XMLMailIdGenerator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public MailIdFile GenerateFile(MailIdRequest request)
        {
            throw new NotImplementedException();
        }

        public MailIdFile GenerateXmlFile(MailIdRequest request)
        {
            var xml = GenerateXmlContent(request);
            var filename = GenerateFileName(request.Header, MailListFileOutputs.XML);

            return new MailIdFile(
                xml,
                $"{request.Header.MailingRef}.xml",
                "application/xml",
                Encoding.GetEncoding("ISO-8859-1")
            );
        }

        private string GenerateXmlContent(MailIdRequest request)
        {
            using var stringWriter = new StringWriter();
            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.GetEncoding("ISO-8859-1"),
                OmitXmlDeclaration = false
            };

            using (var writer = XmlWriter.Create(stringWriter, xmlSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("MailingRequest");

                // Context
                writer.WriteStartElement("Context");
                writer.WriteAttributeString("requestName", "MailingRequest");
                writer.WriteAttributeString("dataset", MailIdProtocolMetadata.Dataset);
                writer.WriteAttributeString("sender", request.Header.SenderId.ToString());
                writer.WriteAttributeString("receiver", MailIdProtocolMetadata.Receiver);
                writer.WriteAttributeString("version", MailIdProtocolMetadata.RequestVersion);
                writer.WriteEndElement(); // Context

                // Header
                writer.WriteStartElement("Header");
                writer.WriteAttributeString("customerId", request.Header.SenderId.ToString());
                writer.WriteAttributeString("accountId", request.Header.AccountId.ToString());
                writer.WriteAttributeString("mode", request.Header.Mode);

                writer.WriteStartElement("Files");
                writer.WriteStartElement("RequestProps");
                writer.WriteAttributeString("customerFileRef", request.Header.MailingRef);
                writer.WriteEndElement(); // RequestProps
                writer.WriteEndElement(); // Files

                writer.WriteEndElement(); // Header

                // MailingCreate
                writer.WriteStartElement("MailingCreate");
                writer.WriteAttributeString("seq", "1");
                writer.WriteAttributeString("mailingRef", request.Header.MailingRef);
                writer.WriteAttributeString("genMID", "N");
                writer.WriteAttributeString("genPSC", "N");
                writer.WriteAttributeString("expectedDeliveryDate", request.Header.ExpectedDeliveryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("type", "MID");
                writer.WriteEndElement();

                // Format
                writer.WriteStartElement("Format");
                writer.WriteString("Small");
                writer.WriteEndElement();

                // Items
                writer.WriteStartElement("Items");
                foreach (var item in request.Items)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteAttributeString("seq", item.Sequence.ToString());
                    writer.WriteAttributeString("lang", item.Language);
                    writer.WriteAttributeString("midNum", item.MailIdNum);
                    writer.WriteAttributeString("priority", item.Priority);

                    writer.WriteStartElement("Comps");
                    foreach (var comp in item.Components)
                    {
                        writer.WriteStartElement("Comp");
                        writer.WriteAttributeString("code", comp.Code.ToString());
                        writer.WriteAttributeString("value", comp.Value);
                        writer.WriteEndElement(); // Comp
                    }
                    writer.WriteEndElement(); // Comps
                    writer.WriteEndElement(); // Item
                }
                writer.WriteEndElement(); // Items

                // ItemCount
                writer.WriteStartElement("ItemCount");
                writer.WriteAttributeString("value", request.ItemCount.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement(); // MailingCreate
                writer.WriteEndElement(); // MailingRequest
                writer.WriteEndDocument();
            }

            return stringWriter.ToString();
        }


        private string GenerateFileName(MailIdRequestHeader header, string fileformat)
        {
            return $"{MailIdProtocolMetadata.FileCode}_{MailIdProtocolMetadata.RequestVersion}_{header.SenderId}_{header.MailingRef}_{_dateTimeProvider.TimeStamp}_{MailIdProtocolMetadata.CommunicationStep}_{fileformat}";
        }
    }
}
