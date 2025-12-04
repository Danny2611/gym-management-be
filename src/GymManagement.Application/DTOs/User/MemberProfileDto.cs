namespace GymManagement.Application.DTOs.User
{
    public class MemberProfileDto
    {
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? AvatarUrl { get; set; }

        public string RoleName { get; set; } = string.Empty;
    }
}
