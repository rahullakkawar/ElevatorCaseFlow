using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElevatorCaseFlow.API.Controllers
{
    /// <summary>
    /// Handles authentication and JWT token generation.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // IConfiguration lets us read from appsettings.json
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// POST /api/auth/login
        /// Validates credentials and returns a JWT token.
        /// Use this token in Swagger's Authorize button to test all endpoints.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simple hardcoded credentials for demo purposes
            // In a real app this would check a database
            if (!IsValidUser(request.Username, request.Password))
                return Unauthorized(new { Message = "Invalid username or password." });

            // Generate JWT token for the authenticated user
            var token = GenerateJwtToken(request.Username);

            return Ok(new
            {
                Token = token,
                ExpiresIn = "60 minutes",
                Message = "Login successful! Copy the token and use it in Swagger Authorize."
            });
        }

        // ── Private Helpers ──

        /// <summary>
        /// Validates username and password.
        /// Hardcoded for demo — real apps use a database.
        /// </summary>
        private bool IsValidUser(string username, string password)
        {
            // Demo credentials — you can change these
            return username == "rahul" && password == "Admin@123";
        }

        /// <summary>
        /// Generates a signed JWT token for the given username.
        /// Token is valid for 60 minutes as configured in appsettings.json.
        /// </summary>
        private string GenerateJwtToken(string username)
        {
            // Read JWT settings from appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiryMins = int.Parse(jwtSettings["ExpiryMinutes"]!);

            // Create signing credentials using the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims are pieces of info stored inside the token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Build the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMins),
                signingCredentials: creds
            );

            // Return token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// Login request body.
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

