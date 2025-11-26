using GymManagement.Application.DTOs.Members;
using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        // 1. Lấy tất cả
        public async Task<List<MemberResponse>> GetAllMembersAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            
            // Map từ Entity -> DTO
            return members.Select(m => new MemberResponse
            {
                Id = m.Id,      
                Name = m.Name,   
                Email = m.Email,
                Phone = m.Phone, 
                Role = m.Role
            }).ToList();
        }

        // 2. Lấy theo ID
        public async Task<MemberResponse> GetMemberByIdAsync(string id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return null;

            return new MemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Role = member.Role
            };
        }

        // 3. Tạo mới (Sửa lỗi AddAsync nếu Repo bạn dùng tên khác)
        public async Task<MemberResponse> CreateMemberAsync(CreateMemberRequest request)
        {
            var newMember = new Member
            {
                // Id thường MongoDB tự tạo, hoặc dùng: ObjectId.GenerateNewId().ToString()
                // Nếu Id là string, không gán Guid.NewGuid() vào đây sẽ gây lỗi CS0029
                Name = request.UserName, // Map từ Request
                Email = request.Email,
                // PasswordHash = request.Password, // Nhớ hash password!
                Phone = request.PhoneNumber,
                Role = "Member"
            };

         await _memberRepository.CreateAsync(newMember);

            return new MemberResponse
            {
                Id = newMember.Id,
                Name = newMember.Name,
                Email = newMember.Email,
                Phone = newMember.Phone,
                Role = newMember.Role
            };
        }

        // 4. Cập nhật
        public async Task<MemberResponse> UpdateMemberAsync(string id, UpdateMemberRequest request)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) throw new KeyNotFoundException("User not found");

            // Cập nhật thông tin
            member.Name = request.UserName; 
            member.Phone = request.PhoneNumber;
            await _memberRepository.UpdateAsync(id, member);
            return new MemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Role = member.Role
            };
        }

        // 5. Xóa
        public async Task<bool> DeleteMemberAsync(string id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return false;

            await _memberRepository.DeleteAsync(id);
            return true;
        }
    }
}