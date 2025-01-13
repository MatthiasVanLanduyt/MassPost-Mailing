namespace Validator.Domain.Mailings.Models
{
    public class MailIdRequest
    {
        public const string VERSION = "0200";
        public const string DATASET = "M037_MID";
        public const string RECEIVER = "MID";

        public required string CustomerId { get; init; }
        public required string AccountId { get; init; }
        public required string Mode { get; init; } // P=Production, T=Test, C=Certification
        public required string MailingRef { get; init; }
        public required string ExpectedDeliveryDate { get; init; }
        public List<MailIdItem> Items { get; set; } = new();
        public int ItemCount => Items.Count;
    }
}
