using System.ComponentModel.DataAnnotations;

namespace GymManagement.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phải từ 2-50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải ít nhất 8 ký tự")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ in hoa và 1 chữ số"
        )]
        public string Password { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
    }
}
