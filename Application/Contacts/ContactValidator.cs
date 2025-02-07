using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Mailings.Models;

namespace Validator.Application.Contacts
{
    public class ContactValidator
    {
        public (bool IsValid, IList<string> Errors) Validate(Contact contact)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(contact.FirstName))
            {
                errors.Add("First name is required");
            }

            if (string.IsNullOrWhiteSpace(contact.LastName))
            {
                errors.Add("Last name is required");
            }

            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                errors.Add("Email is required");
            }
            else if (!IsValidEmail(contact.Email))
            {
                errors.Add("Invalid email address");
            }

            return (!errors.Any(), errors);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
