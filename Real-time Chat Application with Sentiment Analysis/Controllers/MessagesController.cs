using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_time_Chat_Application_with_Sentiment_Analysis.Data;
using Real_time_Chat_Application_with_Sentiment_Analysis.Models;

using System.Security.Claims;

/// <summary>
/// Контроллер для получения сообщений чатов
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все сообщения чата по его Id
    /// </summary>
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var chat = await _context.Chats.FindAsync(chatId);

        if (chat == null)
            return NotFound();

        if (chat.User1Id != userId && chat.User2Id != userId)
            return Forbid();

        var messages = await _context.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new
            {
                user = m.User.Username,
                message = m.Content,
                createdAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    /// <summary>
    /// Получить переписку с конкретным пользователем
    /// </summary>
    [HttpGet("private/{otherUserId}")]
    public async Task<IActionResult> GetPrivateMessages(Guid otherUserId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var chat = await _context.Chats
            .FirstOrDefaultAsync(c =>
                (c.User1Id == userId && c.User2Id == otherUserId) ||
                (c.User1Id == otherUserId && c.User2Id == userId));

        if (chat == null)
            return Ok(new List<object>());

        var messages = await _context.ChatMessages
            .Where(m => m.ChatId == chat.Id)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new
            {
                user = m.User.Username,
                message = m.Content,
                createdAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(messages);
    }
}