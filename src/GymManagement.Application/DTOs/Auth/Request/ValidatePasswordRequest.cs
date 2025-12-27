using System.ComponentModel.DataAnnotations;
namespace GymManagement.Application.DTOs.Auth
{
     public class ValidatePasswordRequest
    {
         [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ in hoa và 1 chữ số"
        )]
        public string CurrentPassword { get; set; }
    }

}
