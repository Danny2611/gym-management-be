// GymManagement.Application/DTOs/User/UpdateEmailRequest.cs
using System;
using System.ComponentModel.DataAnnotations;
namespace GymManagement.Application.DTOs.User
{
    public class UpdateEmailRequest
    {
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
            public string Email { get; set; }
    }
}