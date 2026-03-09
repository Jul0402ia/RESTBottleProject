using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RESTBottleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // configuration hvor den dep.inj. 
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        //using post method since it is guaranteed to have a body (you have to have a body, and that its why you use post, that does have a body)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            // 1. Validate the user (In a real scenario, check your database here)
            // Here we use a simple hardcoded check:
            //normalt ville man ha lavet en service klasse for at komme til db, som ville have ansvar for dette, for at undegå at bruge if 
            if (login.Username == "admin" && login.Password == "1234")
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password.");
        }

        //
        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims are the pieces of information "baked" into the token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin") // You can add roles here
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token is valid for 2 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Helper class to receive JSON data
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
