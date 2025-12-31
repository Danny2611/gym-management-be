namespace GymManagement.Application.DTOs
{
    public class MembershipDetailsResponse
    {
        public string MembershipId { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string MemberAvatar { get; set; } = string.Empty;
        public string PackageId { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string PackageCategory { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public int SessionsRemaining { get; set; }
        public int TotalSessions { get; set; }
    }
}
