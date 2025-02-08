using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Mailings.Models;

namespace WpfDesktop.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; }

        public string AppPropertiesFileName { get; set; }

        public string PrivacyStatement { get; set; }

        public List<Contact> DefaultContacts { get; set; }
    }
}
