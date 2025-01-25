using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdOptions
    {
        public required string GenMid { get; init; } = "N";
        public required string GenPSC { get; init; } = "Y";

        public string Mode { get; init; } = "T"; // P=Production, T=Test, C=Certification
    }
}
