namespace Validator.Domain.Addresses
{
    public record AddressValidationResult
    {
        public ValidationStatus Status { get; init; }

        public List<string> Errors { get; init; } = [];

        public static AddressValidationResult Success => new() { Status = ValidationStatus.Valid };

        public static AddressValidationResult Pending => new() { Status = ValidationStatus.Pending };

        public static AddressValidationResult Invalid(ICollection<string> errors)
        {
            return new AddressValidationResult { Status = ValidationStatus.Invalid, Errors = [.. errors] };
        }

        public override string ToString() =>
            Status switch
            {
                ValidationStatus.Pending => "Pending",
                ValidationStatus.Valid => "Valid",
                ValidationStatus.Invalid => $"Invalid: {string.Join(", ", Errors)}",
                _ => throw new InvalidOperationException()
            };
    }
}
