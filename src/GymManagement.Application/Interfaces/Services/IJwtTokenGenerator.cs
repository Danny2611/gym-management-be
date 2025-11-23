namespace GymManagement.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(string userId, string roleName);
        string GenerateRefreshToken(string userId, string roleName);
        DateTime GetAccessTokenExpiry();
        DateTime GetRefreshTokenExpiry();
    }
}