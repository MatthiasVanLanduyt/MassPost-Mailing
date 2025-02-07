using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Validator.Domain.Mailings.Models;
using WpfDesktop.Contracts.Services;

namespace WpfDesktop.ViewModels
{
    public partial class ContactsViewModel : ObservableObject
    {

        private const string ContactListKey = "ContactList";
        private readonly IPersistAndRestoreService _persistAndRestoreService;

        private readonly ObservableCollection<Contact> contacts = new();
        public IReadOnlyCollection<Contact> Contacts => contacts;

        public ICommand AddContactCommand => _addContactCommand;
        public ICommand DeleteContactCommand => _deleteContactCommand;
        public ICommand SaveCommand => _saveCommand;

        private readonly ICommand _addContactCommand;
        private readonly ICommand _deleteContactCommand;
        private readonly ICommand _saveCommand;

        public ObservableCollection<string> AvailableLanguages { get; } = new()
        {
            "nl",
            "fr",
            "en"
        };

        public ContactsViewModel(IPersistAndRestoreService persistAndRestoreService)
        {
            _addContactCommand = new RelayCommand(AddContact);
            _deleteContactCommand = new RelayCommand<Contact>(DeleteContact);
            _saveCommand = new RelayCommand(SaveContacts);

            _persistAndRestoreService = persistAndRestoreService;

            LoadContacts();
        }

        private void LoadContacts()
        {
            contacts.Clear();

            if (App.Current.Properties.Contains(ContactListKey))
            {
                var savedContacts = App.Current.Properties[ContactListKey] as List<Contact>;
                if (savedContacts != null)
                {
                    foreach (var contact in savedContacts)
                    {
                        contacts.Add(contact);
                    }
                }
            }
        }

        private void SaveContacts()
        {
            App.Current.Properties[ContactListKey] = contacts.ToList();
            _persistAndRestoreService.PersistData();
        }

        private void AddContact()
        {
            contacts.Add(new Contact());
            SaveContacts();
        }

        
        private void DeleteContact(Contact contact)
        {
            if (contact != null)
            {
                contacts.Remove(contact);
            }

            SaveContacts();
        }

        // If you need to handle edited contacts, you can refresh the collection
        private void HandleContactEdited(Contact contact)
        {
            var index = contacts.IndexOf(contact);
            if (index >= 0)
            {
                contacts[index] = contact; // Triggers collection change notification
            }
        }
    }
}
