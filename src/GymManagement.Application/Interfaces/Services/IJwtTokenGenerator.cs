using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Member member);
        DateTime GetTokenExpiry();
    }
}