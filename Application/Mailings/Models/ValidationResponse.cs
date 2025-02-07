using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.MailingResponses.Models;

namespace Validator.Application.Mailings.Models
{
    public record ValidationResponse
    {
        public required string Status { get; init; }

        public string? ErrorMessage { get; init; }

        public List<ValidatedAddress>? ValidatedAddressList { get; init; }
    }
}
