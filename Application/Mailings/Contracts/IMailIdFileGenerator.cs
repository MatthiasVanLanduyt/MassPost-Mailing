using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Contracts
{
    public interface IMailIdFileGenerator
    {
        public MailIdFile GenerateFile(MailIdRequest request);
    }
}