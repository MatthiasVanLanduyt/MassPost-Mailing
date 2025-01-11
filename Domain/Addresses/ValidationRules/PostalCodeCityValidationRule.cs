using System.ComponentModel.DataAnnotations;
using Validator.Domain.Addresses;

namespace Validator.Domain.Addresses.ValidationRules
{
    public class PostalCodeCityValidationRule : IValidationRule
    {
        private readonly Dictionary<string, HashSet<string>> _postalCodeCities;
        public PostalCodeCityValidationRule(Dictionary<string, HashSet<string>> postalCodeCities)
        {
            _postalCodeCities = postalCodeCities;
        }
        public ValidationResult Validate(AddressDetails address)
        {
            if (!_postalCodeCities.TryGetValue(address.PostalCode.ToString(), out var cities) ||
                !cities.Contains(address.City.ToUpperInvariant()))
            {
                return new ValidationResult(AddressErrors.PostalCodeCityMismatch);
            }
            return ValidationResult.Success!;
        }
    }
}