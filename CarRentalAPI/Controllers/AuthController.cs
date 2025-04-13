using CarRentalAPI.DataAccess;
using CarRentalAPI.DTO;
using CarRentalAPI.Entities;
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

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
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
            return Ok(user);
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO request)
        {
            var user = await _authRepository.Login(request);
            if (user == null)
                return Unauthorized("Invalid email or password!");

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
    }
}
