using HMS.ApplicationProj.DTOS;
using HMS.ApplicationProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _auth.RegisterAsync(dto);

            if (!result)
                return BadRequest("User with same PersonalNumber, Email or Phone already exists.");

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var authResponse = await _auth.LoginAsync(dto);

            if (authResponse == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                Token = authResponse.Token,
                Role = authResponse.Role
            });
        }
    }
}