using Microsoft.AspNetCore.Mvc;
using CodeQuestBackend.Services;
using CodeQuestBackend.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using System.Text.Json;

namespace CodeQuestBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DiscordAuthService _discordAuthService;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(DiscordAuthService discordAuthService, IConfiguration configuration, IUserRepository userRepository)
    {
        _discordAuthService = discordAuthService;
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpGet("verify")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult VerifyToken()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            var user = _userRepository.GetUser(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    name = user.Name,
                    avatar = user.Avatar,
                    discordId = user.DiscordId,
                    discordUsername = user.DiscordUsername,
                    role = user.Role,
                    starDustPoints = user.StarDustPoints,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("discord/login")]
    public IActionResult DiscordLogin()
    {
        var clientId = _configuration["Discord:ClientId"];
        var redirectUri = _configuration["Discord:RedirectUri"];
        var scope = "identify email";

        var discordAuthUrl = $"https://discord.com/api/oauth2/authorize" +
            $"?client_id={clientId}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
            $"&response_type=code" +
            $"&scope={scope}";

        return Ok(new { authUrl = discordAuthUrl });
    }

    [HttpGet("discord/callback")]
    public async Task<IActionResult> DiscordCallbackGet([FromQuery] string code, [FromQuery] string? state)
    {
        try
        {
            var redirectUri = _configuration["Discord:RedirectUri"];
            var user = await _discordAuthService.AuthenticateWithDiscordAsync(code, redirectUri!);

            if (user == null)
            {
                return BadRequest(new { message = "Discord authentication failed" });
            }

            var token = GenerateJwtToken(user);

            // Create user data object with all fields
            var userData = new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                name = user.Name,
                avatar = user.Avatar,
                biography = user.Biography,
                birthDate = user.BirthDate?.ToString("yyyy-MM-dd"),
                discordId = user.DiscordId,
                discordUsername = user.DiscordUsername,
                discordDiscriminator = user.DiscordDiscriminator,
                discordAvatar = user.DiscordAvatar,
                role = user.Role,
                starDustPoints = user.StarDustPoints,
                createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            var authData = new { token, user = userData };

            // Return HTML page that will handle the redirect to frontend
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Discord Authentication</title>
    <style>
        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
        .success {{ color: green; }}
        .error {{ color: red; }}
    </style>
</head>
<body>
    <div class='success'>
        <h2>Authentication Successful!</h2>
        <p>You can now close this window and return to the application.</p>
        <script>
            // Send data to parent window if in iframe, otherwise store in localStorage
            const authData = {JsonSerializer.Serialize(authData)};
            
            if (window.opener) {{
                window.opener.postMessage(authData, '*');
                window.close();
            }} else {{
                localStorage.setItem('devtalles_token', authData.token);
                localStorage.setItem('devtalles_user', JSON.stringify(authData.user));
                window.location.href = 'http://localhost:8080';
            }}
        </script>
    </div>
</body>
</html>";

            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            var errorHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Discord Authentication Error</title>
    <style>
        body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
        .error {{ color: red; }}
    </style>
</head>
<body>
    <div class='error'>
        <h2>Authentication Failed</h2>
        <p>Error: {ex.Message}</p>
        <p>Please try again.</p>
    </div>
</body>
</html>";

            return Content(errorHtml, "text/html");
        }
    }

    [HttpPost("discord/callback")]
    public async Task<IActionResult> DiscordCallback([FromBody] DiscordCallbackRequest request)
    {
        try
        {
            // Use the configured redirect URI from settings instead of the one from the request
            var redirectUri = _configuration["Discord:RedirectUri"];
            var user = await _discordAuthService.AuthenticateWithDiscordAsync(request.Code, redirectUri!);

            if (user == null)
            {
                return BadRequest(new { message = "Discord authentication failed" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    name = user.Name,
                    avatar = user.Avatar,
                    biography = user.Biography,
                    birthDate = user.BirthDate?.ToString("yyyy-MM-dd"),
                    discordId = user.DiscordId,
                    discordUsername = user.DiscordUsername,
                    discordDiscriminator = user.DiscordDiscriminator,
                    discordAvatar = user.DiscordAvatar,
                    role = user.Role,
                    starDustPoints = user.StarDustPoints,
                    createdAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            Console.WriteLine($"Token refresh request received. Token present: {!string.IsNullOrEmpty(request.Token)}");
            
            // Validate the JWT token first and get the user from the token claims
            var userId = GetUserIdFromToken(request.Token);
            Console.WriteLine($"Extracted user ID from token: {userId}");
            
            if (userId == null)
            {
                Console.WriteLine("Invalid token - no user ID found");
                return Unauthorized(new { message = "Invalid token" });
            }

            // Get the user from the database
            var user = _userRepository.GetUser(userId.Value);
            Console.WriteLine($"User found in database: {user != null}");
            
            if (user == null)
            {
                Console.WriteLine("User not found in database");
                return Unauthorized(new { message = "User not found" });
            }

            // Generate a new JWT token
            var jwtToken = GenerateJwtToken(user);
            Console.WriteLine("New JWT token generated successfully");

            return Ok(new
            {
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    name = user.Name,
                    avatar = user.Avatar,
                    discordId = user.DiscordId,
                    discordUsername = user.DiscordUsername,
                    role = user.Role,
                    starDustPoints = user.StarDustPoints,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpPost("discord/refresh")]
    public async Task<IActionResult> RefreshDiscordToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validate the JWT token first and get the user from the token claims
            var userId = GetUserIdFromToken(request.Token);
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Get the user from the database
            var user = _userRepository.GetUser(userId.Value);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            // Refresh the Discord token
            var newAccessToken = await _discordAuthService.RefreshDiscordTokenAsync(user);
            if (newAccessToken == null)
            {
                return BadRequest(new { message = "Failed to refresh Discord token" });
            }

            // Generate a new JWT token
            var jwtToken = GenerateJwtToken(user);

            return Ok(new
            {
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    name = user.Name,
                    avatar = user.Avatar,
                    discordId = user.DiscordId,
                    discordUsername = user.DiscordUsername,
                    role = user.Role,
                    starDustPoints = user.StarDustPoints,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }


    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("discord_id", user.DiscordId ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "User"),
            new Claim("created_at", user.CreatedAt.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}

public class DiscordCallbackRequest
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}
