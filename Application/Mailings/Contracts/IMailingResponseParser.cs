using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.MailingResponses.Models;

namespace Validator.Application.Mailings.Contracts
{
    public interface IMailingResponseParser
    {
        public MailingResponse ParseResponse(Stream stream);
    }
}
