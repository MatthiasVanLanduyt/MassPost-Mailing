using Validator.Application.Mailings.Models;
using Validator.Domain.MailingResponses.Models;

namespace Validator.Application.Mailings.Contracts
{
    public interface IMailingResponseCsvExporter
    {
        MailResponseFile ExportToCsv(MailingResponse response);
    }
}
