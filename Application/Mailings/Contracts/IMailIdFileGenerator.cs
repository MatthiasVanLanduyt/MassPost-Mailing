using Validator.Application.Mailings.Models;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings.Contracts
{
    public interface IMailIdFileGenerator
    {
        MailIdFile GenerateFile(MailIdRequest request);
    }
}