using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.Admin
{
    public static class MemberMappings
    {
        public static MemberResponseDto ToDto(this Member member, Role role)
        {
            return new MemberResponseDto
            {
                Id = member.Id,
                Name = member.Name,
                Avatar = member.Avatar,
                Email = member.Email,
                Gender = member.Gender,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth,
                Address = member.Address,
                Role = role != null ? new MemberRoleDto
                {
                    Id = role.Id,
                    Name = role.Name
                } : null,
                Status = member.Status,
                IsVerified = member.IsVerified,
                CreatedAt = member.CreatedAt,
                UpdatedAt = member.UpdatedAt
            };
        }

        public static Member ToEntity(this CreateMemberDto dto, string roleId, string hashedPassword)
        {
            return new Member
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Gender = dto.Gender,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                Role = roleId,
                Status = dto.Status ?? "active",
                IsVerified = dto.IsVerified ?? false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}