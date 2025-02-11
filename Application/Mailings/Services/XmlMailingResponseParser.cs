using System.Text.Json;
using System.Xml.Linq;
using Validator.Application.Mailings.Contracts;
using Validator.Domain.MailingResponses.Models;

namespace Validator.Domain.MailingResponses.Services
{
    public class XmlMailingResponseParser : IMailingResponseParser
    {
        private readonly Dictionary<string, StatusCode> _statusCodes;

        public XmlMailingResponseParser()
        {

            // Load status codes at startup
            //var path = Path.Combine(applicationPath, "Addresses", "BelgianPostalCodes.json");
            _statusCodes = LoadStatusCodes();
        }

        private Dictionary<string, StatusCode> LoadStatusCodes()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(appDirectory, "Mailings", "statuscodes.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Status codes file not found at {filePath}");
            }

            var json = File.ReadAllText(filePath);
            var codes = JsonSerializer.Deserialize<List<StatusCode>>(json);
            return codes.ToDictionary(x => x.Code);
        }
        public MailingResponse ParseResponse(Stream stream)
        {
            var response = new MailingResponse();
            var doc = XDocument.Load(stream);

            var header = doc.Descendants("Header").First();

            response.Header = new MailingResponseHeader
            {
                CustomerId = header.Attribute("customerId")?.Value ?? "",
                AccountId = header.Attribute("accountId")?.Value ?? "",
                Mode = header.Attribute("mode")?.Value ?? "",
                CustomerFileRef = header
                                    .Descendants("RequestProps")?
                                    .First()
                                    .Attribute("customerFileRef")?
                                    .Value ?? ""
            };

            var mailingCreate = doc.Descendants("MailingCreate").First();

            // Get overall status
            response.Status = mailingCreate.Element("Status")?.Attribute("code")?.Value ?? "";
            response.MailingRef = mailingCreate.Attribute("mailingRef")?.Value ?? "";

            // Get compliance rates from first Reply
            var firstReply = mailingCreate.Descendants("Reply").First();
            var messageContents = firstReply.Descendants("MessageContent");

            foreach (var content in messageContents)
            {
                var key = content.Attribute("key")?.Value;
                var value = content.Attribute("value")?.Value;

                if (key == null || value == null) continue;

                // Parse percentage value
                var percentageValue = ParsePercentage(value);

                switch (key)
                {
                    case "compliancyRateAtBuildingLevel":
                        response.BuildingComplianceRate = percentageValue;
                        break;
                    case "presortingCodeCompliancyRate":
                        response.PresortingComplianceRate = percentageValue;
                        break;
                    case "addressesWithRecipientCompliancyRate":
                        response.AddressComplianceRate = percentageValue;
                        break;
                }
            }

            // Parse individual address responses
            var addressReplies = mailingCreate.Descendants("Reply")
                .Where(r => r.Element("Locations")?
                             .Element("Location")?
                             .Attribute("tagName")?
                             .Value == "Item");

            foreach (var reply in addressReplies)
            {
                var addressResponse = new AddressResponse();

                // Get sequence from Location element
                var sequence = reply.Element("Locations")?
                                   .Element("Location")?
                                   .Attribute("attributeValue")?
                                   .Value;

                if (int.TryParse(sequence, out int seq))
                {
                    addressResponse.Sequence = seq;
                }

                // Parse messages
                var messages = reply.Element("Messages")?.Elements("Message") ?? Enumerable.Empty<XElement>();
                foreach (var message in messages)
                {

                    var statusCode = message.Attribute("code")?.Value;

                    var responseMessage = new AddressResponseMessage
                    {
                        StatusCode = statusCode ?? "",
                        Severity = message.Attribute("severity")?.Value ?? "",
                        MessageContents = new List<MessageContent>()
                    };

                    // Add status code description if available
                    if (statusCode != null && _statusCodes.TryGetValue(statusCode, out var status))
                    {
                        responseMessage.Description = status.Description;
                    }
                    
                    // Parse message contents
                    var contents = message.Element("MessageContents")?.Elements("MessageContent") ??
                                 Enumerable.Empty<XElement>();

                    foreach (var content in contents)
                    {
                        responseMessage.MessageContents.Add(new MessageContent
                        {
                            Key = content.Attribute("key")?.Value ?? "",
                            Value = content.Attribute("value")?.Value ?? ""
                        });

                        // Get presorting code if present
                        if (content.Attribute("key")?.Value == "psCode")
                        {
                            addressResponse.PreSortingCode = content.Attribute("value")?.Value;
                        }
                    }

                    addressResponse.Messages.Add(responseMessage);
                }

                response.AddressResponses.Add(addressResponse);
            }

            return response;
        }

        private static decimal ParsePercentage(string value)
        {
            // Remove '%' character and trim whitespace
            var cleanValue = value.Replace("%", "").Trim();

            // Try to parse as decimal
            if (decimal.TryParse(cleanValue,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimal result)) 
            { 
                return result; 
            }

                // Return 0 if parsing fails
                return 0;
        }
    }
}
