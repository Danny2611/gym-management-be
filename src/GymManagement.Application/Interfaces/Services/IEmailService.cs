namespace GymManagement.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string email, string otp);
        Task SendWelcomeEmailAsync(string email, string name);
    }
}