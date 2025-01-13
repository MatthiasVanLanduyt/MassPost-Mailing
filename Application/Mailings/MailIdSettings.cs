using System.Text.Json;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Mailings
{
    public class MailIdSettings
    {
        private static readonly string DefaultConfigPath = "defaultSettings.json";
        public readonly string FileCode = "MID";
        public readonly string RequestVersion = "0200";
        public readonly string CommunicationStep = "0RQ";
        public readonly string CommunicationType = "MID";
        public readonly string Dataset = "M037_MID";
        public readonly string Mode = "P";

        // Properties with default values
        public string OutputPath { get; set; } = @"C:\Users\vanlanm\OneDrive - pacar\Projects\Mailingman - MassPost";
        public string SerialNumber { get; set; } = "MM12345678";
        public string FileFormat = MailListFileOutputs.TXT;
        public int CustomerId = 4493;
        public int AccountId = 73771;


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
