using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.MailingResponses.Models
{
    public class AddressResponse
    {
        public int Sequence { get; set; }
        public string PreSortingCode { get; set; }
        public string Status { get; set; }
        public List<AddressResponseMessage> Messages { get; set; } = [];
    }

}
