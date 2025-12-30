namespace GymManagement.Application.DTOs
{
    public class MembershipDetailsResponse
    {
        public string MembershipId { get; set; }
        public string MemberName { get; set; }
        public string MemberAvatar { get; set; }
        public string PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageCategory { get; set; }
        public string Status { get; set; }
        public int DaysRemaining { get; set; }
        public int SessionsRemaining { get; set; }
        public int TotalSessions { get; set; }
    }
}
