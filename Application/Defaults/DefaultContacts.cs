using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Defaults
{        
    public static class DefaultContacts
    {
            public static IReadOnlyList<Contact> GetDefaults() => new List<Contact>
        {
            new Contact
            {
                FirstName = "Matthias",
                LastName = "Van Landuyt",
                Email = "matthias@pacar.be",
                Phone = "056 37 19 80",
                Mobile = "056 37 33 60",
                LanguageCode = "nl"
            },
         
        };

    }
}
