using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDesktop.Models;

namespace WpfDesktop.Contracts.Services
{
    public interface ISettingsService
    {
        public MailingSettings GetMailingSettings();
        public void SaveMailingSettings(MailingSettings settings);
        
    }
}
