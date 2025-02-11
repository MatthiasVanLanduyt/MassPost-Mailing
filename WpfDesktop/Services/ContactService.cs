using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Options;
using Validator.Domain.Mailings.Models;
using WpfDesktop.Contracts;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;

namespace WpfDesktop.Services
{
   
    public class ContactService : IContactService
    {
        private readonly string _contactsFilePath;
        private readonly AppConfig _appConfig;

        public ContactService(string appDataPath, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _contactsFilePath = Path.Combine(appDataPath, AppConstants.ContactsFileName);
            EnsureContactsExist();
        }

        private void EnsureContactsExist()
        {
            try
            {
                if (!File.Exists(_contactsFilePath))
                {
                    // If no contacts file exists, save the default contacts from config
                    SaveContacts(_appConfig.DefaultContacts ?? new List<Contact>());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ensuring contacts exist: {ex.Message}");
                // Don't throw - we'll create contacts when needed
            }
        }

        public List<Contact> GetContacts()
        {
            try
            {
                if (File.Exists(_contactsFilePath))
                {
                    var json = File.ReadAllText(_contactsFilePath);
                    return JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading contacts: {ex.Message}");
            }

            return new List<Contact>();
        }

        public void SaveContacts(List<Contact> contacts)
        {
            try
            {
                var directory = Path.GetDirectoryName(_contactsFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_contactsFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving contacts: {ex.Message}");
                throw; // Rethrow so UI can handle the error
            }
        }
    }
}
