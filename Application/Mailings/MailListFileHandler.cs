using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharedkernel;

namespace Validator.Application.Mailings
{
    public class MailListFileGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public MailListFileGenerator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        private string GenerateFileName(MailIdSettings settings)
        {
            return $"{settings.FileCode}_{settings.RequestVersion}_{settings.SerialNumber}_{_dateTimeProvider.TimeStamp}_{settings.CommunicationStep}_{settings.FileFormat}";
        }


    }
}
