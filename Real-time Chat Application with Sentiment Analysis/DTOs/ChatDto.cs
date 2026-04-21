namespace Real_time_Chat_Application_with_Sentiment_Analysis.DTOs
{
    public class ChatDto
    {
        public Guid ChatId { get; set; }
        public string Username { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}
