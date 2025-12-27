using System.ComponentModel.DataAnnotations;
namespace GymManagement.Application.DTOs.Auth
{
    public class ResendOTPRequest
    {
         [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }
}