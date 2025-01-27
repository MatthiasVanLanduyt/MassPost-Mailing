using System.Xml.Linq;
using Validator.Domain.MailingResponses.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Domain.MailingResponses.Services
{
    public class XmlMailingResponseParser
    {

        public static MailingResponse ParseResponse(string xmlContent)
        {
            var response = new MailingResponse();
            var doc = XDocument.Parse(xmlContent);
            var mailingCreate = doc.Descendants("MailingCreate").First();

            // Get overall status
            response.Status = mailingCreate.Element("Status")?.Attribute("code")?.Value ?? "";

            // Get compliance rates from first Reply
            var firstReply = mailingCreate.Descendants("Reply").First();
            var messageContents = firstReply.Descendants("MessageContent");

            foreach (var content in messageContents)
            {
                var key = content.Attribute("key")?.Value;
                var value = content.Attribute("value")?.Value;

                if (key == null || value == null) continue;

                switch (key)
                {
                    case "compliancyRateAtBuildingLevel":
                        response.BuildingComplianceRate = value;
                        break;
                    case "presortingCodeCompliancyRate":
                        response.PresortingComplianceRate = value;
                        break;
                    case "addressesWithRecipientCompliancyRate":
                        response.AddressComplianceRate = value;
                        break;
                }
            }

            return response;
        }
    }
    

}
