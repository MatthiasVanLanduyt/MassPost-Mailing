using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses.ValidationRules;

namespace Validator.Domain.Addresses
{
    public class AddressValidator
    {

        private readonly IEnumerable<IValidationRule> _validationRules;
        public AddressValidator(IPostalCodeService postalCodeService)
        {
            _validationRules = new List<IValidationRule>
            {
                new PostalCodeValidationRule(postalCodeService.PostalCodeCities),
                new PostalCodeCityValidationRule(postalCodeService.PostalCodeCities),
                new SpecialCharactersValidationRule()
            };
        }

        public AddressValidationResult ValidateAddress(AddressDetails addressDetails)
        {
            var validationErrors = new List<string>();

            foreach (var rule in _validationRules)
            {
                var result = rule.Validate(addressDetails);
                Debug.WriteLine($"Rule: {rule.ToString()} Result: {result}");

                if (result != ValidationResult.Success && result.ErrorMessage != null)
                {
                    validationErrors.Add(result.ErrorMessage);
                }
            }

            return validationErrors.Count == 0
                ? AddressValidationResult.Success
                : AddressValidationResult.Invalid(validationErrors);
        }

        public async Task ValidateAddressLinesAsync(IEnumerable<AddressLine> addressLines)
        {
            // Parallel validation of address lines
            await Parallel.ForEachAsync(addressLines, async (line, token) =>
            {
                line.Validation = ValidateAddress(line.AddressDetails);
            });
        }

    }
}
