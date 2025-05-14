using CarRentalAPI.DataAccess;
using CarRentalAPI.DTO;
using CarRentalAPI.Entities;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly LogService _logService;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration, LogService logService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _logService = logService;
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO request)
        {
            // Kullanıcı zaten var mı kontrol et
            if (await _authRepository.UserExists(request.Email))
            {
                return BadRequest("User already exists.");
            }

            var user = await _authRepository.Register(request);

            _logService.AddLog(user.User_ID, "User Registered", $"Email: {user.Email}");

            return Ok(user);
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO request)
        {
            var user = await _authRepository.Login(request);
            if (user == null)
                return Unauthorized("Invalid email or password!");

            _logService.AddLog(user.User_ID, "User Logged In", $"Email: {user.Email}");

            // Token oluştur
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.User_ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] int userId)
        {
            _logService.AddLog(userId, "User Logged Out", $"User with ID: {userId} logged out.");
            return Ok(new { message = "User logged out and log recorded." });
        }

    }
}
