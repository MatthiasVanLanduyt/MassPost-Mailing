using System.Collections;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Validator.Domain.Mailings.Models;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;

namespace WpfDesktop.Services
{
    public class PersistAndRestoreService : IPersistAndRestoreService
    {
        private readonly IFileService _fileService;
        private readonly AppConfig _appConfig;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private readonly Dictionary<string, Type> _knownTypes = new()
        {
            { "ContactList", typeof(List<Contact>) }
            // Add other known types here if needed
        };

        public PersistAndRestoreService(IFileService fileService, IOptions<AppConfig> appConfig)
        {
            _fileService = fileService;
            _appConfig = appConfig.Value;

            if (string.IsNullOrEmpty(_appConfig.ConfigurationsFolder) || string.IsNullOrEmpty(_appConfig.AppPropertiesFileName))
            {
                throw new ArgumentException("AppConfig properties are not properly initialized.");
            }

            InitializeDefaultProperties();

            RestoreData();
        }

        private void InitializeDefaultProperties()
        {
            if (!App.Current.Properties.Contains("ContactList"))
            {
                // Use the contacts from configuration
                App.Current.Properties["ContactList"] = _appConfig.DefaultContacts;
            }
        }

        public void PersistData()
        {
            if (App.Current.Properties != null)
            {
                var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
                var fileName = _appConfig.AppPropertiesFileName;

                // Convert any List<T> to proper objects before saving
                var propertiesToSave = new Dictionary<object, object>();

                foreach (DictionaryEntry entry in App.Current.Properties)
                {
                    var value = entry.Value;
                    if (value != null && value.GetType().IsGenericType &&
                        value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        // Serialize and deserialize to ensure proper object structure
                        value = JsonSerializer.Deserialize(
                            JsonSerializer.Serialize(value),
                            value.GetType());
                    }
                    propertiesToSave[entry.Key] = value;
                }

                _fileService.Save(folderPath, fileName, propertiesToSave);
            }
        }

        public void RestoreData()
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            var fileName = _appConfig.AppPropertiesFileName;
            var properties = _fileService.Read<IDictionary>(folderPath, fileName);

            if (properties != null)
            {
                foreach (DictionaryEntry entry in properties)
                {
                    var key = entry.Key.ToString();
                    var value = entry.Value;

                    if (_knownTypes.TryGetValue(key, out Type targetType))
                    {
                        var deserializedValue = JsonSerializer.Deserialize(value.ToString(), targetType);
                        App.Current.Properties[key] = deserializedValue;
                    }
                                       
                    else
                    {
                        App.Current.Properties[key] = value;
                    }
                }
            }
        }

    }
}
