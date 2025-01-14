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

        


    }
}
