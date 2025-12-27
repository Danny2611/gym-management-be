using System.ComponentModel.DataAnnotations;

namespace GymManagement.Application.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ in hoa và 1 chữ số"
        )]
        public string NewPassword { get; set; }
    }
}
