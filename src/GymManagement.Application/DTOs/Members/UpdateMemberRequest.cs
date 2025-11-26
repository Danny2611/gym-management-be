namespace GymManagement.Application.DTOs.Members
{
    public class UpdateMemberRequest
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        // Không cho update Email hoặc Password ở API này cho an toàn
    }
}