namespace Real_time_Chat_Application_with_Sentiment_Analysis.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
