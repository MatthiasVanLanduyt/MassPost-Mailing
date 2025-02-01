using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdOptions
    {
        public required bool GenMid { get; init; } = false;
        public required bool GenPSC { get; init; } = true;

        public string Mode { get; init; } = "T"; // P=Production, T=Test, C=Certification
    }
}
