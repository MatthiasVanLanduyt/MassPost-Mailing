using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfDesktop.Contracts;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;

namespace WpfDesktop.Services
{
    public class SettingsService : ISettingsService
    {

        private readonly IPersistAndRestoreService _persistAndRestoreService;

        public SettingsService(IPersistAndRestoreService persistAndRestoreService)
        {
            _persistAndRestoreService = persistAndRestoreService;
            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings()
        {
            if (!Application.Current.Properties.Contains(AppConstants.MailingSettingsKey))
            {
                Application.Current.Properties[AppConstants.MailingSettingsKey] = new MailingSettings();
                _persistAndRestoreService.PersistData();
            }
        }

        public MailingSettings GetMailingSettings()
        {
            return Application.Current.Properties[AppConstants.MailingSettingsKey] as MailingSettings ?? new MailingSettings();
        }   

        public void SaveMailingSettings(MailingSettings settings)
        {
            Application.Current.Properties[AppConstants.MailingSettingsKey] = settings;
            _persistAndRestoreService.PersistData();
        }
    }
}
