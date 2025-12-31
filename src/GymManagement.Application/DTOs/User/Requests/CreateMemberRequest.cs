// GymManagement.Application/DTOs/User/Requests/CreateMemberRequest.cs
using System.ComponentModel.DataAnnotations;

namespace GymManagement.Application.DTOs.User
{
    public class CreateMemberRequest
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [RegularExpression("^(male|female|other)$",
            ErrorMessage = "Giới tính chỉ chấp nhận: male, female hoặc other")]
        public string? Gender { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? Address { get; set; }

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Role không được để trống")]
        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = "active";
    }
}
