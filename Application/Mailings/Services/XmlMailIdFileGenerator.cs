using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sharedkernel;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Services
{
    public class XmlMailIdFileGenerator : IMailIdFileGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public XmlMailIdFileGenerator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public MailIdFile GenerateFile(MailIdRequest request)
        {
            MemoryStream stream = new MemoryStream();

            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.GetEncoding("ISO-8859-1"),
                OmitXmlDeclaration = false
            };

            using (var writer = XmlWriter.Create(stream, xmlSettings))
            {
                WriteXmlContent(writer, request);
            }

            var filename = GenerateFileName(request.Header, MailListFileOutputs.XML);

            return new MailIdFile(
                stream.ToArray(),
                filename,
                "application/xml"
            );
        }

        private static void WriteXmlContent(XmlWriter writer, MailIdRequest request)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("MailingRequest");

            WriteContext(writer, request);
            WriteHeader(writer, request);
            WriteMailingCreate(writer, request);

            writer.WriteEndElement(); // MailingRequest
            writer.WriteEndDocument();

        }

        private static void WriteContext(XmlWriter writer, MailIdRequest request)
        {
            writer.WriteStartElement("Context");
                writer.WriteAttributeString("requestName", "MailingRequest");
                writer.WriteAttributeString("dataset", MailIdProtocolMetadata.Dataset);
                writer.WriteAttributeString("sender", request.Header.SenderId.ToString());
                writer.WriteAttributeString("receiver", MailIdProtocolMetadata.Receiver);
                writer.WriteAttributeString("version", MailIdProtocolMetadata.RequestVersion);
            writer.WriteEndElement();
        }

        private static void WriteHeader(XmlWriter writer, MailIdRequest request)
        {
            writer.WriteStartElement("Header");
            writer.WriteAttributeString("customerId", request.Header.SenderId.ToString());
            writer.WriteAttributeString("accountId", request.Header.AccountId.ToString());
            writer.WriteAttributeString("mode", request.Options.Mode);

            writer.WriteStartElement("Files");
            writer.WriteStartElement("RequestProps");
            writer.WriteAttributeString("customerFileRef", request.Header.CustomerFileRef);
            writer.WriteEndElement(); // RequestProps
            writer.WriteEndElement(); // Files
            writer.WriteEndElement(); // Header
        }

        private static void WriteMailingCreate(XmlWriter writer, MailIdRequest request)
        {
            writer.WriteStartElement("MailingCreate");
            writer.WriteAttributeString("seq", "1");
            writer.WriteAttributeString("mailingRef", request.Header.MailingRef);
            writer.WriteAttributeString("genMID", request.Options.GenMid ? "Y" : "N");
            writer.WriteAttributeString("genPSC", request.Options.GenPSC ? "Y" : "N");
            writer.WriteAttributeString("expectedDeliveryDate", request.Header.ExpectedDeliveryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            // FileInfo
            writer.WriteStartElement("FileInfo");

            writer.WriteAttributeString("type", MailIdProtocolMetadata.FileInfoCode);
            writer.WriteEndElement();

            // Format
            writer.WriteStartElement("Format");
            writer.WriteString(request.MailFormat);
            writer.WriteEndElement();
            
            WriteContacts(writer, request);
            
            WriteItems(writer, request);
            
            writer.WriteEndElement(); // MailingCreate
        }
                    
        private static void WriteItems(XmlWriter writer, MailIdRequest request)
        {
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
                    //if comp.value is null or empty dont write the attribute

                    if (String.IsNullOrEmpty(comp.Value)) continue;

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
        }

        private static void WriteContacts(XmlWriter writer, MailIdRequest request)
        {
            //Contacts
            int index = 1;
            writer.WriteStartElement("Contacts");
            foreach (var contact in request.Contacts)
            {
                writer.WriteStartElement("Contact");

                writer.WriteAttributeString("seq", index.ToString());
                writer.WriteAttributeString("firstName", contact.FirstName);
                writer.WriteAttributeString("lastName", contact.LastName);
                writer.WriteAttributeString("email", contact.Email);
                writer.WriteAttributeString("lang", contact.LanguageCode);
                writer.WriteAttributeString("phone", contact.Phone);
                writer.WriteAttributeString("mobile", contact.Mobile);
                writer.WriteEndElement(); // Contact
                index++;
            }
            writer.WriteEndElement(); // Contacts   
        }
   
        private string GenerateFileName(MailIdRequestHeader header, string fileformat)
        {
            return $"{MailIdProtocolMetadata.Receiver}_{MailIdProtocolMetadata.RequestVersion}_{header.SenderId}_{header.CustomerFileRef}_{_dateTimeProvider.TimeStamp}_{MailIdProtocolMetadata.CommunicationStep}.{fileformat}";
        }
    }
}
