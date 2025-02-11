using System.Diagnostics;
using System.IO;
using System.Text.Json;
using WpfDesktop.Contracts;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;

namespace WpfDesktop.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        public SettingsService(string appDataPath)
        {
            _settingsFilePath = Path.Combine(appDataPath, AppConstants.MailingSettingsFileName);
            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    SaveMailingSettings(new MailingSettings());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ensuring settings exist: {ex.Message}");
                // Don't throw - we'll create settings when needed
            }
        }

        public MailingSettings GetMailingSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    return JsonSerializer.Deserialize<MailingSettings>(json) ?? new MailingSettings();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading settings: {ex.Message}");
            }

            return new MailingSettings();
        }   

        public void SaveMailingSettings(MailingSettings settings)
        {
            try
            {
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving settings: {ex.Message}");
                throw; // Rethrow so UI can handle the error
            }
        }
    }
}
