using System.ComponentModel.DataAnnotations;
using Validator.Domain.Addresses;

namespace Validator.Domain.Addresses.ValidationRules
{
    public class PostalCodeValidationRule : IValidationRule
    {
        private readonly Dictionary<string, HashSet<string>> _postalCodeCities;

        public PostalCodeValidationRule(Dictionary<string, HashSet<string>> postalCodeCities)
        {
            _postalCodeCities = postalCodeCities;
        }

        public ValidationResult Validate(AddressDetails address)
        {
            if (string.IsNullOrEmpty(address.PostalCode.ToString()))
                return new ValidationResult(AddressErrors.PostalCodeIsEmpty);

            if (address.PostalCode.ToString().Length != 4)
                return new ValidationResult(AddressErrors.PostalCodeIsIncorrectLength);

            if (!_postalCodeCities.ContainsKey(address.PostalCode.ToString()))
                return new ValidationResult(AddressErrors.PostalCodeDoesNotExist);

            return ValidationResult.Success;
        }
    }
}