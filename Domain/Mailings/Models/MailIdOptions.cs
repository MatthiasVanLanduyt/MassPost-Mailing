using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Models
{
    public record MailIdOptions
    {
        public required bool GenMid { get; init; }
        public required bool GenPsc { get; init; }

        public required string Mode { get; init; }
    }
}
