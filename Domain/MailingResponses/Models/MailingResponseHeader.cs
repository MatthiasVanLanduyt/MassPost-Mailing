using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public record MailingResponseHeader
    {
        public string CustomerId;
        public string AccountId;
        public string CustomerFileRef;
        public string Mode;
    }
}
