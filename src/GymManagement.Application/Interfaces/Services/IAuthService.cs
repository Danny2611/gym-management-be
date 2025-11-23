using GymManagement.Application.DTOs.Auth;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> VerifyOTPAsync(VerifyOTPRequest request);
        Task ResendOTPAsync(string email);
    }
}