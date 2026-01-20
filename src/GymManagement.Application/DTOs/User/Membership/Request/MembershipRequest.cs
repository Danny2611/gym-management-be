namespace GymManagement.Application.DTOs.User
{
    // Request đăng ký gói tập
    public class RegisterPackageRequest
    {
        public string PackageId { get; set; }
    }

    // Request pause/resume membership
    public class MembershipActionRequest
    {
        public string MembershipId { get; set; }
    }


}