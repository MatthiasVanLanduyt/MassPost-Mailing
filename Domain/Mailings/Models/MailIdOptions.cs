using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdOptions
    {
        public required string DepositId { get; init; } = "";
        public required string DepositIdentifierType { get; init; } = "N";
        public required string GenMid { get; init; } = "N";
        public required string GenPSC { get; init; } = "Y";
    }
}
