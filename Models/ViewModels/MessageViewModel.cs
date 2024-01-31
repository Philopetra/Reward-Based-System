namespace RYT.Models.ViewModels
{
    public class MessageViewModel
    {
        public string PhotoUrl { get; set; }
        public string LastText { get; set; }
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime ReadOn { get; set; }
        public DateTime DeliverOn { get; set; }

        public List<MessageThread> MessageThreads { get; set; }

    }

    public class MessageThread
    {
        public string PhotoUrl { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }

    }
}

