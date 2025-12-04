namespace GymManagement.Application.DTOs.Auth
{
    public class VerifyOTPRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}