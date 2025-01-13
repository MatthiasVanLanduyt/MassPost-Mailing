using System.IO;
using System.Text;
using System.Xml;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings
{
    // Generator for creating the MAIL ID file
    public class MailIdFileGenerator
    {
        public MailIdFile GenerateTxtFile(MailIdRequest request)
        {
            string content = GenerateTxtContent(request);

            return new MailIdFile(content, $"{request.MailingRef}.txt", "text/plain", Encoding.UTF8);

        }
        private string GenerateTxtContent(MailIdRequest request)
        {
            var lines = new List<string>
            {
                // Header section
                $"Context|MailingRequest|{MailIdRequest.DATASET}|{request.CustomerId}|{MailIdRequest.RECEIVER}|{MailIdRequest.VERSION}",
                $"Header|{request.CustomerId}|{request.AccountId}|{request.Mode}",
                $"MailingCreate|1|{request.MailingRef}|||N|N|{request.ExpectedDeliveryDate}",
                "FileInfo|MID",
                "Format|Small"
            };

            // Add items
            foreach (var item in request.Items)
            {
                lines.Add($"Item|{item.Sequence}|{item.Language}|{item.MailIdNum}||{item.Priority}");

                foreach (var comp in item.Components)
                {
                    lines.Add($"Comp|{comp.Code}|{comp.Value}");
                }
            }

            lines.Add($"ItemCount|{request.ItemCount}");

            return string.Join(Environment.NewLine, lines);
        }

        public void SaveTxtFile(MailIdRequest request, string outputPath)
        {
            string content = GenerateTxtContent(request);
            File.WriteAllText(outputPath, content);
        }

        public MailIdFile GenerateXmlFile(MailIdRequest request)
        {
            var xml = GenerateXmlContent(request);

            return new MailIdFile(
                xml,
                $"{request.MailingRef}.xml",
                "application/xml",
                Encoding.GetEncoding("ISO-8859-1")
            );
        }

        public string GenerateXmlContent(MailIdRequest request)
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
                writer.WriteAttributeString("dataset", MailIdRequest.DATASET);
                writer.WriteAttributeString("sender", request.CustomerId);
                writer.WriteAttributeString("receiver", MailIdRequest.RECEIVER);
                writer.WriteAttributeString("version", MailIdRequest.VERSION);
                writer.WriteEndElement(); // Context

                // Header
                writer.WriteStartElement("Header");
                writer.WriteAttributeString("customerId", request.CustomerId);
                writer.WriteAttributeString("accountId", request.AccountId);
                writer.WriteAttributeString("mode", request.Mode);

                writer.WriteStartElement("Files");
                writer.WriteStartElement("RequestProps");
                writer.WriteAttributeString("customerFileRef", request.MailingRef);
                writer.WriteEndElement(); // RequestProps
                writer.WriteEndElement(); // Files

                writer.WriteEndElement(); // Header

                // MailingCreate
                writer.WriteStartElement("MailingCreate");
                writer.WriteAttributeString("seq", "1");
                writer.WriteAttributeString("mailingRef", request.MailingRef);
                writer.WriteAttributeString("genMID", "N");
                writer.WriteAttributeString("genPSC", "N");
                writer.WriteAttributeString("expectedDeliveryDate", request.ExpectedDeliveryDate);

                // FileInfo
                writer.WriteStartElement("FileInfo");
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

        public void SaveXmlFile(MailIdRequest request, string outputPath)
        {
            string content = GenerateXmlContent(request);
            File.WriteAllText(outputPath, content, Encoding.GetEncoding("ISO-8859-1"));
        }
    }
}
