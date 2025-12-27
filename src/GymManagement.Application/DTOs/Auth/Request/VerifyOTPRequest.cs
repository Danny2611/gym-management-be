using System.ComponentModel.DataAnnotations;

namespace GymManagement.Application.DTOs.Auth
{
    public class VerifyOTPRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP không được để trống")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có 6 ký tự")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP phải là số")]
        public string Otp { get; set; } = string.Empty;
    }
}
