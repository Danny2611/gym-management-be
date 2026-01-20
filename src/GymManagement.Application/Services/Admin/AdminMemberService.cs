using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Mappings.Admin;
namespace GymManagement.Application.Services.Admin
{


    public class AdminMemberService : IAdminMemberService
    {
        private readonly IAdminMemberRepository _memberRepository;
        private readonly IRoleRepository _roleRepository;

        public AdminMemberService(
            IAdminMemberRepository memberRepository,
            IRoleRepository roleRepository)
        {
            _memberRepository = memberRepository;
            _roleRepository = roleRepository;
        }

        public async Task<MemberListResponseDto> GetAllMembersAsync(MemberQueryOptions options)
        {
            var (members, totalCount) = await _memberRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            // Populate roles for all members
            var memberDtos = new List<MemberResponseDto>();
            foreach (var member in members)
            {
                var role = await _roleRepository.GetByIdAsync(member.Role);
                memberDtos.Add(member.ToDto(role));
            }

            return new MemberListResponseDto
            {
                Members = memberDtos,
                TotalMembers = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<MemberResponseDto> GetMemberByIdAsync(string memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);

            if (member == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hội viên");
            }

            var role = await _roleRepository.GetByIdAsync(member.Role);
            return member.ToDto(role);
        }

        public async Task<MemberResponseDto> CreateMemberAsync(CreateMemberDto dto)
        {
            // Check if email already exists
            var existingMember = await _memberRepository.GetByEmailAsync(dto.Email);
            if (existingMember != null)
            {
                throw new InvalidOperationException("Email đã tồn tại");
            }

            // Get default role (Member)
            var defaultRole = await _roleRepository.GetByNameAsync("Member");
            if (defaultRole == null)
            {
                throw new InvalidOperationException("Không tìm thấy vai trò mặc định");
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create member entity
            var member = new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Gender = dto.Gender,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                Role = defaultRole.Id,
                Status = dto.Status ?? "active",
                IsVerified = dto.IsVerified ?? false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdMember = await _memberRepository.CreateAsync(member);
            return createdMember.ToDto(defaultRole);
        }

        public async Task<MemberResponseDto> UpdateMemberAsync(string memberId, UpdateMemberDto dto)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hội viên");
            }

            // Check if email is being updated and already exists
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != member.Email)
            {
                var existingMember = await _memberRepository.GetByEmailAsync(dto.Email);
                if (existingMember != null)
                {
                    throw new InvalidOperationException("Email đã tồn tại");
                }
            }

            // Validate role if provided
            Role updatedRole = null;
            if (!string.IsNullOrEmpty(dto.RoleId))
            {
                updatedRole = await _roleRepository.GetByIdAsync(dto.RoleId);
                if (updatedRole == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy Role");
                }
                member.Role = dto.RoleId;
            }

            // Update fields
            if (!string.IsNullOrEmpty(dto.Name))
                member.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Avatar))
                member.Avatar = dto.Avatar;

            if (!string.IsNullOrEmpty(dto.Email))
                member.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Gender))
                member.Gender = dto.Gender;

            if (!string.IsNullOrEmpty(dto.Phone))
                member.Phone = dto.Phone;

            if (dto.DateOfBirth.HasValue)
                member.DateOfBirth = dto.DateOfBirth;

            if (!string.IsNullOrEmpty(dto.Address))
                member.Address = dto.Address;

            if (dto.IsVerified.HasValue)
                member.IsVerified = dto.IsVerified.Value;

            var updatedMember = await _memberRepository.UpdateAsync(memberId, member);

            // Get role for response
            var role = updatedRole ?? await _roleRepository.GetByIdAsync(updatedMember.Role);
            return updatedMember.ToDto(role);
        }

        public async Task<MemberResponseDto> UpdateMemberStatusAsync(string memberId, string status)
        {
            // Validate status
            var validStatuses = new[] { "active", "inactive", "pending", "banned" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Trạng thái không hợp lệ");
            }

            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hội viên");
            }

            var updatedMember = await _memberRepository.UpdateStatusAsync(memberId, status);
            var role = await _roleRepository.GetByIdAsync(updatedMember.Role);
            return updatedMember.ToDto(role);
        }

        public async Task<bool> DeleteMemberAsync(string memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hội viên");
            }

            return await _memberRepository.DeleteAsync(memberId);
        }

        public async Task<MemberStatsDto> GetMemberStatsAsync()
        {
            return await _memberRepository.GetStatsAsync();
        }
    }
}