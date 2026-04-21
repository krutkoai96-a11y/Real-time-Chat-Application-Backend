using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Real_time_Chat_Application_with_Sentiment_Analysis.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
