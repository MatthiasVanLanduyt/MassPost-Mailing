using System.Security.Cryptography.X509Certificates;

namespace Validator.Domain.Mailings.Models
{
    public class MailIdRequest
    {
        
        public required MailIdRequestHeader Header { get; init; }
        public required MailIdOptions Options { get; init; }
        public required string MailFormat { get; init; } = MailFormats.SmallFormat;
        public required string MailFileInfo { get;init; } = MailingTypes.MailId;

        public required List<Contact> Contacts { get; init; }

        public List<MailIdItem> Items { get; set; } = new();
        public int ItemCount => Items.Count;
    }
}
