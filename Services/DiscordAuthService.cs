using System.Text.Json;
using CodeQuestBackend.Models;
using CodeQuestBackend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CodeQuestBackend.Services;

public class DiscordAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public DiscordAuthService(IUserRepository userRepository, IConfiguration configuration, HttpClient httpClient)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<User?> AuthenticateWithDiscordAsync(string code, string redirectUri)
    {
        try
        {
            // Exchange code for access token
            var tokenResponse = await ExchangeCodeForTokenAsync(code, redirectUri);
            if (tokenResponse == null)
                return null;

            // Get user info from Discord
            var discordUser = await GetDiscordUserInfoAsync(tokenResponse.AccessToken);
            if (discordUser == null)
                return null;

            // Find or create user
            var user = await FindOrCreateUserAsync(discordUser, tokenResponse);

            return user;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Discord authentication error: {ex.Message}");
            return null;
        }
    }

    private async Task<DiscordTokenResponse?> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        var clientId = _configuration["Discord:ClientId"];
        var clientSecret = _configuration["Discord:ClientSecret"];

        var parameters = new Dictionary<string, string>
        {
            {"client_id", clientId!},
            {"client_secret", clientSecret!},
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri}
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await _httpClient.PostAsync("https://discord.com/api/oauth2/token", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Discord token exchange error: {response.StatusCode} - {errorContent}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Discord token response: {json}");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(json, options);
            if (tokenResponse != null)
            {
                Console.WriteLine($"Parsed token - AccessToken: {tokenResponse.AccessToken?.Substring(0, Math.Min(20, tokenResponse.AccessToken?.Length ?? 0))}..., TokenType: {tokenResponse.TokenType}, ExpiresIn: {tokenResponse.ExpiresIn}");
            }
            else
            {
                Console.WriteLine("Failed to deserialize token response - tokenResponse is null");
            }

            return tokenResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing token response: {ex.Message}");
            return null;
        }
    }

    private async Task<DiscordUser?> GetDiscordUserInfoAsync(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("Access token is null or empty");
            return null;
        }

        Console.WriteLine($"Attempting to get Discord user info with token: {accessToken.Substring(0, Math.Min(20, accessToken.Length))}...");

        var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        Console.WriteLine($"Request headers: Authorization = Bearer {accessToken.Substring(0, Math.Min(20, accessToken.Length))}...");

        var response = await _httpClient.SendAsync(request);

        Console.WriteLine($"Discord API response: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Discord API error: {response.StatusCode} - {errorContent}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Discord user info response: {json}");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var discordUser = JsonSerializer.Deserialize<DiscordUser>(json, options);
            if (discordUser != null)
            {
                Console.WriteLine($"Parsed Discord user - ID: {discordUser.Id}, Username: {discordUser.Username}, Email: {discordUser.Email}");
            }
            else
            {
                Console.WriteLine("Failed to deserialize Discord user - discordUser is null");
            }

            return discordUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing Discord user: {ex.Message}");
            return null;
        }
    }

    private async Task<User> FindOrCreateUserAsync(DiscordUser discordUser, DiscordTokenResponse tokenResponse)
    {
        // Try to find existing user by Discord ID
        var existingUser = await _userRepository.GetByDiscordIdAsync(discordUser.Id);

        if (existingUser != null)
        {
            // Update existing user with new token info
            existingUser.DiscordAccessToken = tokenResponse.AccessToken;
            existingUser.DiscordRefreshToken = tokenResponse.RefreshToken;
            existingUser.DiscordTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            existingUser.DiscordUsername = discordUser.Username;
            existingUser.DiscordDiscriminator = discordUser.Discriminator;
            existingUser.DiscordAvatar = discordUser.Avatar;

            await _userRepository.UpdateAsync(existingUser);
            return existingUser;
        }

        // Create new user
        var newUser = new User
        {
            DiscordId = discordUser.Id,
            DiscordUsername = discordUser.Username,
            DiscordDiscriminator = discordUser.Discriminator,
            DiscordAvatar = discordUser.Avatar,
            DiscordAccessToken = tokenResponse.AccessToken,
            DiscordRefreshToken = tokenResponse.RefreshToken,
            DiscordTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            Email = discordUser.Email,
            Username = discordUser.Username,
            Name = discordUser.GlobalName ?? discordUser.Username,
            Avatar = GetDiscordAvatarUrl(discordUser.Id, discordUser.Avatar),
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            StarDustPoints = 0
        };

        await _userRepository.CreateAsync(newUser);
        return newUser;
    }

    private string? GetDiscordAvatarUrl(string discordId, string? avatarHash)
    {
        if (string.IsNullOrEmpty(avatarHash))
            return $"https://cdn.discordapp.com/embed/avatars/{int.Parse(discordId) % 5}.png";

        return $"https://cdn.discordapp.com/avatars/{discordId}/{avatarHash}.png";
    }

    public async Task<string?> RefreshDiscordTokenAsync(User user)
    {
        if (string.IsNullOrEmpty(user.DiscordRefreshToken))
            return null;

        try
        {
            var clientId = _configuration["Discord:ClientId"];
            var clientSecret = _configuration["Discord:ClientSecret"];

            var parameters = new Dictionary<string, string>
            {
                {"client_id", clientId!},
                {"client_secret", clientSecret!},
                {"grant_type", "refresh_token"},
                {"refresh_token", user.DiscordRefreshToken}
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync("https://discord.com/api/oauth2/token", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(json);

            if (tokenResponse != null)
            {
                user.DiscordAccessToken = tokenResponse.AccessToken;
                user.DiscordRefreshToken = tokenResponse.RefreshToken ?? user.DiscordRefreshToken;
                user.DiscordTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                await _userRepository.UpdateAsync(user);
                return tokenResponse.AccessToken;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Discord token refresh error: {ex.Message}");
            return null;
        }
    }
}

public class DiscordTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public string Scope { get; set; } = string.Empty;
}

public class DiscordUser
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Discriminator { get; set; } = string.Empty;
    public string? GlobalName { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public bool Verified { get; set; }
}
