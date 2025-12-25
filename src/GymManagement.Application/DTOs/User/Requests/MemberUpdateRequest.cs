// GymManagement.Application/DTOs/User/Request/MemberUpdateRequest.cs
using System;
using System.ComponentModel.DataAnnotations;
namespace GymManagement.Application.DTOs.User
{
    public class MemberUpdateRequest
    {
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string? Name { get; set; }

        [RegularExpression("^(male|female|other)$",
            ErrorMessage = "Giới tính chỉ chấp nhận: male, female hoặc other")]
        public string? Gender { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        public string? Email { get; set; }
    }
}