using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_chat.Dtos;
using dotnet_chat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_chat.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JsonFileService _jsonFileService;
        private readonly IConfiguration _configuration;

        public AuthController(JsonFileService jsonFileService, IConfiguration configuration)
        {
            _jsonFileService = jsonFileService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDTO user)
        {
            if (_jsonFileService.GetUser(user.Username) != null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _jsonFileService.AddUser(user);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(UserDTO user)
        {
            var dbUser = _jsonFileService.GetUser(user.Username);
            if (dbUser == null)
            {
                return Unauthorized(new { message = "User not registered" });
            }

            if (!BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user.Username);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
