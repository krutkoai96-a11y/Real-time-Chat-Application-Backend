namespace Real_time_Chat_Application_with_Sentiment_Analysis.Controllers
{
    using Real_time_Chat_Application_with_Sentiment_Analysis.DTOs;
    using Real_time_Chat_Application_with_Sentiment_Analysis.Services;
    using Microsoft.AspNetCore.Mvc;

    namespace ChatApi.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly AuthService _authService;

            public AuthController(AuthService authService)
            {
                _authService = authService;
            }

            [HttpPost("register")]
            public IActionResult Register(RegisterDto dto)
            {
                try
                {
                    var token = _authService.Register(dto.Username, dto.Password);
                    return Ok(new { token });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginDto dto)
            {
                try
                {
                    var token = await _authService.Login(dto.Username, dto.Password);
                    return Ok(new { token });
                }
                catch
                {
                    return Unauthorized();
                }
            }
        }
    }
}
