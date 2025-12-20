namespace GymManagement.Application.DTOs.User
{
    public class MemberUpdateRequest
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
}
