using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public record MessageContent
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}
