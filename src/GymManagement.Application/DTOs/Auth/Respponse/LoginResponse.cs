namespace GymManagement.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public UserInfo User { get; set; }
        public TokenInfo Tokens { get; set; }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Role ID
        public string RoleName { get; set; } // Role Name (Admin, Member, etc.)
        public string Avatar { get; set; }
    }

    public class TokenInfo
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}