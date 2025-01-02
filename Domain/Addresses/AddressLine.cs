namespace Validator.Domain.Addresses
{
    public class AddressLine
    {
        public required string Contact { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }

        public override string ToString()
        {
            return $"Address: {Street},{PostalCode} {City} ";
        }

    }
}
