// GymManagement.Application/DTOs/User/UploadAvatarDto.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GymManagement.Application.DTOs.User
{
    public class UploadAvatarDto
    {
        [Required(ErrorMessage = "File avatar không được để trống")]
        public IFormFile Avatar { get; set; }
    }
}