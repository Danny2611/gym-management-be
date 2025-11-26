namespace GymManagement.Application.DTOs.Members
{
    public class MemberResponse
    {
        // Dùng string vì MongoDB và dự án bạn đang dùng string cho ID
        public string Id { get; set; } 
        
        // Sửa UserName -> Name (theo lỗi CS1061 trong ảnh)
        public string Name { get; set; } 
        
        public string Email { get; set; }
        
        // Sửa PhoneNumber -> Phone (theo file RegisterRequest của bạn)
        public string Phone { get; set; } 
        
        public string Role { get; set; }
    }
}