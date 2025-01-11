namespace Validator.Domain.Addresses
{
    public class AddressLine
    {
        public int Index { get; init; }
        public AddressDetails AddressDetails { get; set; }
        public AddressValidationResult Validation { get; set; }

        public AddressLine(AddressDetails addressDetails, int index)
        {
            Index = index;
            AddressDetails = addressDetails;
            Validation = AddressValidationResult.Pending;
        }

        public override string ToString() =>
            $"Line {Index}: {AddressDetails}, Status: {Validation.Status}";
    }
}

