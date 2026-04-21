namespace Real_time_Chat_Application_with_Sentiment_Analysis.Models
{
    public class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid User1Id { get; set; }
        public User User1 { get; set; }

        public Guid User2Id { get; set; }
        public User User2 { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}
