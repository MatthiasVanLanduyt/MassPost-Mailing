using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public class AddressResponseMessage
    {
        public string StatusCode { get; set; }
        public string Severity { get; set; }
        
        public List<MessageContent> MessageContents { get; set; }

    }
}
