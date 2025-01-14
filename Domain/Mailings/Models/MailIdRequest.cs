namespace Validator.Domain.Mailings.Models
{
    public class MailIdRequest
    {
        public required MailIdRequestHeader Header { get; init; }
        
        public List<MailIdItem> Items { get; set; } = new();
        public int ItemCount => Items.Count;
    }
}
