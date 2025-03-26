using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public record AddressResponseMessage
    {
        public required string StatusCode { get; set; }
        public required string Severity { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<MessageContent> MessageContents { get; set; } = [];

    }
}
