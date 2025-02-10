using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Mailings.Models;

namespace WpfDesktop.Contracts.Services
{
    public interface IContactService
    {
        List<Contact> GetContacts();
        void SaveContacts(List<Contact> contacts);
    }

}
