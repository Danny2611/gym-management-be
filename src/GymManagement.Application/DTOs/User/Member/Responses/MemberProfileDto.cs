// GymManagement.Application/DTOs/User/MemberProfileDto.cs
namespace GymManagement.Application.DTOs.User
{
    public class MemberProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public string? Gender { get; set; }
        public string Status { get; set; }
        public bool IsVerified { get; set; }
        public string RoleName { get; set; }
    }
}