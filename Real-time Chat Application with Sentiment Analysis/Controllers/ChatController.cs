using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_time_Chat_Application_with_Sentiment_Analysis.Data;
using Real_time_Chat_Application_with_Sentiment_Analysis.Models;

using System.Security.Claims;

/// <summary>
/// Контроллер для работы со списком чатов пользователя
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChatsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить список чатов текущего пользователя
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyChats()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var chats = await _context.Chats
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .Select(c => new
            {
                chatId = c.Id,
                username = c.User1Id == userId ? c.User2.Username : c.User1.Username,
                lastMessage = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                lastMessageTime = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefault()
            })
            .OrderByDescending(c => c.lastMessageTime)
            .ToListAsync();

        return Ok(chats);
    }
}