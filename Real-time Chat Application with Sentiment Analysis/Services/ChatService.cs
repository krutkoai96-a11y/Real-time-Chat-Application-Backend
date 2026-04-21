using Microsoft.EntityFrameworkCore;
using Real_time_Chat_Application_with_Sentiment_Analysis.Data;
using Real_time_Chat_Application_with_Sentiment_Analysis.Models;

namespace Real_time_Chat_Application_with_Sentiment_Analysis.Services
{
    public class ChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отправка сообщения пользователю (создаёт чат если нужно)
        /// </summary>
        public async Task<ChatMessage> SavePrivateMessage(string senderId, string receiverId, string content)
        {
            if (!Guid.TryParse(senderId, out var senderGuid) ||
                !Guid.TryParse(receiverId, out var receiverGuid))
                throw new Exception("Invalid user id");

            // 🔍 ищем чат
            var chat = await _context.Chats
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == senderGuid && c.User2Id == receiverGuid) ||
                    (c.User1Id == receiverGuid && c.User2Id == senderGuid));

            // ➕ если нет — создаём
            if (chat == null)
            {
                chat = new Chat
                {
                    Id = Guid.NewGuid(),
                    User1Id = senderGuid,
                    User2Id = receiverGuid
                };

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatId = chat.Id,
                UserId = senderGuid,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        /// <summary>
        /// Отправка сообщения в существующий чат
        /// </summary>
        public async Task<ChatMessage> SaveMessageToChat(Guid chatId, string senderId, string content)
        {
            if (!Guid.TryParse(senderId, out var senderGuid))
                throw new Exception("Invalid sender id");

            var chat = await _context.Chats
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
                throw new Exception("Chat not found");

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatId = chat.Id,
                UserId = senderGuid,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }
    }
}