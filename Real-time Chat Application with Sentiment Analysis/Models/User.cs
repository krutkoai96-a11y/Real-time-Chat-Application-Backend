namespace Real_time_Chat_Application_with_Sentiment_Analysis.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}
