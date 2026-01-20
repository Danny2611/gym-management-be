using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Auth
{
    // Request để bắt đầu OAuth flow
    public class SocialLoginRequest
    {
        [JsonPropertyName("callback_url")]
        public string? CallbackUrl { get; set; }
    }

    // Response từ OAuth callback
    public class SocialAuthCallbackResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("user_data")]
        public UserInfo UserData { get; set; }
    }

    // External login info từ Google/Facebook
    public class ExternalAuthInfo
    {
        public string ProviderId { get; set; } // Google ID hoặc Facebook ID
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string Provider { get; set; } // "Google" hoặc "Facebook"
    }
}