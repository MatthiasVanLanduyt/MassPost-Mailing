using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.MailingResponses.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Services
{
    public class MergeAddressValidationService
    {
        public List<ValidatedAddress> Merge(MailIdRequest request, MailingResponse response)
            {
            // Verify mailing references match
            if (request.Header.CustomerFileRef != response.Header.CustomerFileRef)
            {
                throw new InvalidOperationException($"File references do not match. Request: {request.Header.CustomerFileRef}, Response: {response.Header.CustomerFileRef}");
            }

            var validatedAddresses = new List<ValidatedAddress>();

            // Join request and response based on sequence number
            var addressMatches = request.Items
                    .Join(response.AddressResponses,
                        req => req.Sequence,
                        resp => resp.Sequence,
                        (req, resp) => new { RequestAddress = req, ResponseAddress = resp });

            foreach (var match in addressMatches)
                {
                    // Check for warnings or errors in response messages
                    var responseMessages = match.ResponseAddress.Messages;

                    if (responseMessages.Count == 0) continue;

                // Get most severe message (ERROR > WARN > INFO)
                var mostSevereMessage = responseMessages
                            .OrderByDescending(m => GetSeverityRank(m.Severity))
                            .First();

                        var validatedAddress = new ValidatedAddress
                        {
                            AddressDetails = match.RequestAddress.AddressDetails,
                            StatusCode = mostSevereMessage.StatusCode,
                            Severity = mostSevereMessage.Severity,
                            Message = mostSevereMessage.Description,
                        };
                validatedAddresses.Add(validatedAddress);
            }
                return validatedAddresses;
            }

            private int GetSeverityRank(string severity)
            {
                return severity?.ToUpper() switch
                {
                    "ERROR" => 3,
                    "WARN" => 2,
                    "INFO" => 1,
                    _ => 0
                };
            }
        }
}
