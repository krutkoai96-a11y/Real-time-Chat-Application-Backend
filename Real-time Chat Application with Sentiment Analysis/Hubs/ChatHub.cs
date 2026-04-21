using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Real_time_Chat_Application_with_Sentiment_Analysis.Services;
using System.Security.Claims;

namespace Real_time_Chat_Application_with_Sentiment_Analysis.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// При подключении — добавляем пользователя в его чаты (группы)
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                // ⚠️ здесь можно добавить загрузку чатов и AddToGroup
                // пока просто лог
                Console.WriteLine($"User connected: {userId}");
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Отправка сообщения новому пользователю
        /// </summary>
        public async Task SendPrivateMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderName))
                return;

            if (string.IsNullOrEmpty(receiverId))
                throw new Exception("receiverId is NULL");

            var msg = await _chatService.SavePrivateMessage(senderId, receiverId, message);

            var payload = new
            {
                chatId = msg.ChatId,
                user = senderName,
                message,
                createdAt = msg.CreatedAt
            };

            // 🔥 отправляем обоим
            await Clients.User(receiverId).SendAsync("ReceiveMessage", payload);
            await Clients.User(senderId).SendAsync("ReceiveMessage", payload);
        }

        /// <summary>
        /// Отправка сообщения в существующий чат
        /// </summary>
        public async Task SendMessage(string chatId, string message)
        {
            var senderId = Context.UserIdentifier;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (!Guid.TryParse(chatId, out var chatGuid))
                throw new Exception($"Invalid chatId: {chatId}");

            var msg = await _chatService.SaveMessageToChat(chatGuid, senderId, message);

            var payload = new
            {
                chatId = msg.ChatId,
                user = senderName,
                message,
                createdAt = msg.CreatedAt
            };

            // 🔥 пока шлём напрямую пользователям (без групп)
            await Clients.All.SendAsync("ReceiveMessage", payload);
        }
    }
}