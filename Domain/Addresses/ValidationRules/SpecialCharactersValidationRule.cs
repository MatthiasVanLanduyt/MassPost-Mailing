using System.ComponentModel.DataAnnotations;
using Validator.Domain.Addresses;

namespace Validator.Domain.Addresses.ValidationRules
{
    public class SpecialCharactersValidationRule : IValidationRule
    {
        private readonly HashSet<char> _specialCharacters;
        public SpecialCharactersValidationRule()
        {
            _specialCharacters = new("!@#$%^&*()_+={}[]|\\:;\"'<>?~`".ToCharArray());
        }
        public ValidationResult Validate(AddressDetails address)
        {
            var fieldsToCheck = new Dictionary<string, string>
            {
                {"Contact", address.Contact},
                {"Street", address.Street},
                {"Postal code", address.PostalCode.ToString()},
                {"City", address.City}
            };
            foreach (var field in fieldsToCheck)
            {
                var invalidChars = field.Value
                    .Where(c => _specialCharacters.Contains(c))
                    .Distinct()
                    .ToList();
                if (invalidChars.Count > 0)
                {
                    return new ValidationResult(
                        $"{field.Key} contains invalid character(s): {string.Join(", ", invalidChars)}");
                }
            }
            return ValidationResult.Success!;
        }
    }
}