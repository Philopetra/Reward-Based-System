namespace RYT.Models.Entities
{
    public class Message: BaseEntity
    {
        public string Text { get; set; }
        public string UserId { get; set; }

        // Navigation prop
        public AppUser User { get; set; }
    }
}
