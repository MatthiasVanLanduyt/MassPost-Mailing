using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Validator.Domain.Mailings.Models;
using WpfDesktop.Contracts;
using WpfDesktop.Contracts.Services;

namespace WpfDesktop.Services
{
   
    public class ContactService : IContactService
    {
        private readonly IPersistAndRestoreService _persistAndRestoreService;

        public ContactService(IPersistAndRestoreService persistAndRestoreService)
        {
            _persistAndRestoreService = persistAndRestoreService;
        }

        public List<Contact> GetContacts()
        {
            return Application.Current.Properties[AppConstants.ContactListKey] as List<Contact> ?? new List<Contact>();
        }

        public void SaveContacts(List<Contact> contacts)
        {
            Application.Current.Properties[AppConstants.ContactListKey] = contacts;
            _persistAndRestoreService.PersistData();
        }
    }
}
