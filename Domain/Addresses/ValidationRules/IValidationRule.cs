using System.ComponentModel.DataAnnotations;
using Validator.Domain.Addresses;

namespace Validator.Domain.Addresses.ValidationRules
{
    public interface IValidationRule
    {
        ValidationResult Validate(AddressDetails address);
    }
}