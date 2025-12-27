namespace GymManagement.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string email, string otp);
        Task SendWelcomeEmailAsync(string email, string name);

        Task SendChangeEmailOtpAsync(string email, string otp);

        Task SendResetPasswordOtpAsync(string email, string otp);
        Task SendPasswordChangedNotificationAsync(string email, string name);
    }
}