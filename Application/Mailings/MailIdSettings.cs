using System.Text.Json;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings
{
    public class MailIdSettings
    {
        private static readonly string DefaultConfigPath = "defaultSettings.json";

        // Properties with default values
        public string OutputPath { get; set; } = @"C:\Users\vanlanm\OneDrive - pacar\Projects\Mailingman - MassPost";      

        // Load defaults from JSON
        public static MailIdSettings LoadDefaults()
        {
           if (File.Exists(DefaultConfigPath))
           {
                try
                {
                    string json = File.ReadAllText(DefaultConfigPath);
                    return JsonSerializer.Deserialize<MailIdSettings>(json)
                           ?? new MailIdSettings();
                }
                catch
                {
                    return new MailIdSettings();
                }
            }
            return new MailIdSettings();
        }

        public void SaveAsDefaults()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(DefaultConfigPath, json);
        }
    }
}
