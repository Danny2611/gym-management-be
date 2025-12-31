namespace GymManagement.Application.DTOs.User.Responses
{
    public class MembershipResponse
    {
        public string Id { get; set; }
        public string MemberId { get; set; }
        public PackageInfo PackageId { get; set; }
        public PaymentInfo PaymentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public string Status { get; set; }
        public int AvailableSessions { get; set; }
        public int UsedSessions { get; set; }
        public DateTime LastSessionsReset { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PackageInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public List<string> Benefits { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public bool Popular { get; set; }
        public int TrainingSessions { get; set; }
        public int SessionDuration { get; set; }
    }

    public class PaymentInfo
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
