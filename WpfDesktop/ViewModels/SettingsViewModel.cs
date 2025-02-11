using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Validator.Domain.Mailings.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;
using Wpf.Ui.Extensions;

namespace WpfDesktop.ViewModels
{
    public partial class SettingsViewModel: ObservableObject
    {

        [ObservableProperty]
        private MailingSettings mailingSettings;

        public List<string> OutputFormatOptions { get; } = [MailListFileOutputs.XML, MailListFileOutputs.TXT];
        public List<string> ModeOptions { get; } = ["T", "P", "C"]; // Test, Production, Certification
        public List<string> SortingModeOptions { get; } = ["Customer Order (CU)", "Print Order (PO)"];
        public List<string> PriorityOptions { get; } = new() { "NP", "P" };
        public List<string> LanguageOptions { get; } = new() { "nl", "fr", "en" };

        private readonly ISettingsService _settingsService;
        private readonly ISnackbarService _snackbarService;

        public ICommand SaveCommand => _saveCommand;
        public ICommand ResetCommand => _resetCommand;
        public ICommand BrowseFolderCommand => _browseFolderCommand;

        private readonly ICommand _saveCommand;
        private readonly ICommand _resetCommand;
        private readonly ICommand _browseFolderCommand;

        public SettingsViewModel(ISettingsService settingsService, ISnackbarService snackbarService)
        {
            _settingsService = settingsService;
            _snackbarService = snackbarService;

            _saveCommand = new RelayCommand(SaveSettings);
            _resetCommand = new RelayCommand(ResetToDefaults);
            _browseFolderCommand = new RelayCommand(BrowseFolder);

            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                MailingSettings = _settingsService.GetMailingSettings();
                
            }
            catch (Exception ex)
            {
                // Handle loading error
                MailingSettings = new MailingSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                _settingsService.SaveMailingSettings(mailingSettings);

                _snackbarService.Show(
                    "Settings saved",
                    "Settings saved successfully",
                    ControlAppearance.Success,
                    new TimeSpan(0, 0, 2));
            }
            catch (Exception ex)
            {
                // Handle saving error
                _snackbarService.Show(
                    "Error saving settings",
                    "An error occurred while saving settings",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 2));
            }
        }

        private void ResetToDefaults()
        {
            MailingSettings = new MailingSettings();
        }

        private void BrowseFolder()
        {
            var dialog = new OpenFolderDialog
            {
                InitialDirectory = MailingSettings.DefaultSaveLocation ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true)
            {
                
                MailingSettings.DefaultSaveLocation = dialog.FolderName;

                _snackbarService.Show(
                    "Default folder change",
                    $"Default folder changed to {dialog.FolderName}\nDon't forget to save settings",
                    ControlAppearance.Info,
                    new TimeSpan(0, 0, 2));

                OnPropertyChanged(nameof(MailingSettings));
            }
        }

    }
}
