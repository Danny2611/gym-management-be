using GymManagement.Application.DTOs.Auth;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> VerifyOTPAsync(VerifyOTPRequest request);
        Task ResendOTPAsync(string email);

        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordRequest request);
        Task VerifyOTPForgotPasswordAsync(VerifyOTPRequest request);
        Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<bool> ValidateCurrentPasswordAsync(string userId, string currentPassword);
        Task<GoogleCallbackResponse> GoogleCallbackAsync(string googleUserId, string email, string name, string? avatar);

        
    }
}