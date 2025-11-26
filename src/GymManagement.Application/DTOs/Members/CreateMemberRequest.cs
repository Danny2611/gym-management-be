namespace GymManagement.Application.DTOs.Members
{
    public class CreateMemberRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Chỉ dùng khi tạo
        public string PhoneNumber { get; set; }
    }
}