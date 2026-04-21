namespace Real_time_Chat_Application_with_Sentiment_Analysis.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Real_time_Chat_Application_with_Sentiment_Analysis.Data;
    using System.Security.Claims;

    
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // 🔥 берём пользователя из БД
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                id = user.Id,
                username = user.Username
            });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized();

            var users = await _context.Users
                .Where(u => u.Id.ToString() != currentUserId)
                .Select(u => new
                {
                    id = u.Id,
                    username = u.Username
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
