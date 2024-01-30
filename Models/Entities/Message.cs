namespace RYT.Models.Entities
{
    public class Message: BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public string UserId { get; set; }= string.Empty;
        public DateTime DeliverdOn { get; set; } = DateTime.UtcNow;
        public string MessageId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public DateTime ReadOn { get; set; } = DateTime.UtcNow;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        // Navigation prop
        public AppUser? User { get; set; }
    }
}
