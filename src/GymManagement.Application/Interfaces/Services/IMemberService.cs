using GymManagement.Application.DTOs.Members; // Nhớ using DTO
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IMemberService
    {
        // 1. Lấy danh sách (Trả về DTO thay vì Entity)
        Task<List<MemberResponse>> GetAllMembersAsync();

        // 2. Lấy chi tiết
        Task<MemberResponse> GetMemberByIdAsync(string id);

        // 3. Tạo mới (Dùng Request DTO để hứng dữ liệu từ form)
        Task<MemberResponse> CreateMemberAsync(CreateMemberRequest request);

        // 4. Cập nhật (Cái bạn đang thiếu)
        Task<MemberResponse> UpdateMemberAsync(string id, UpdateMemberRequest request);

        // 5. Xóa (Cái bạn đang thiếu)
        Task<bool> DeleteMemberAsync(string id);
    }
}