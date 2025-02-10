using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Validator.Application.Contacts;
using Validator.Domain.Mailings.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfDesktop.Contracts.Services;

namespace WpfDesktop.ViewModels
{
    public partial class ContactsViewModel : ObservableObject
    {

        private readonly ISnackbarService _snackbarService;
        private readonly IContactService _contactService;
        private readonly ContactValidator _validator;


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

        public ContactsViewModel(IContactService contactService, ISnackbarService snackbarService)
        {
            _addContactCommand = new RelayCommand(AddContact);
            _deleteContactCommand = new RelayCommand<Contact>(DeleteContact);
            _saveCommand = new RelayCommand(SaveContacts);

            _contactService = contactService;
            _snackbarService = snackbarService;
            _validator = new ContactValidator();

            LoadContacts();
        }

        private void LoadContacts()
        {
            contacts.Clear();

            var savedContacts = _contactService.GetContacts();
            
            foreach (var contact in savedContacts)
            {
                contacts.Add(contact);
            }

        }

        private void SaveContacts()
        {
            
            var invalidContacts = new List<(Contact Contact, IList<string> Errors)>();

            // Validate all contacts
            foreach (var contact in contacts)
            {
                var (isValid, errors) = _validator.Validate(contact);
                if (!isValid)
                {
                    invalidContacts.Add((contact, errors));
                }
            }

            if (invalidContacts.Any())
            {
                // Format error message
                var errorMessage = new StringBuilder("Please fix the following issues:\n");
                foreach (var (contact, errors) in invalidContacts)
                {
                    errorMessage.AppendLine($"{contact.FirstName} {contact.LastName}:");
                    foreach (var error in errors)
                    {
                        errorMessage.AppendLine($"- {error}");
                    }
                }

                _snackbarService.Show(
                    "Validation Error",
                    errorMessage.ToString(),
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 5));


                return;
            }

            _contactService.SaveContacts(contacts.ToList());

            _snackbarService.Show(
                   "Contacts saved",
                   "Contacts saved successfully",
                   ControlAppearance.Success,
                   new TimeSpan(0, 0, 2));
        }

        private void AddContact()
        {
            contacts.Add(new Contact());
        }

        
        private void DeleteContact(Contact contact)
        {
            if (contact != null)
            {
                contacts.Remove(contact);
            }
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
