// using GymManagement.Application.DTOs.Member;
// using GymManagement.Domain.Entities;

// namespace GymManagement.Application.Mappings.user
// {
//     public static class MemberMappings
//     {
//         public static MemberResponseDto ToDto(this Member member)
//         {
//             return new MemberResponseDto
//             {
//                 Id = member.Id,
//                 Name = member.Name,
//                 Avatar = member.Avatar,
//                 Email = member.Email,
//                 Gender = member.Gender,
//                 Phone = member.Phone,
//                 DateOfBirth = member.DateOfBirth,
//                 Address = member.Address,
//                 Role = member.Role != null ? new MemberRoleDto
//                 {
//                     Id = member.Role.Id,
//                     Name = member.Role.Name
//                 } : null,
//                 Status = member.Status,
//                 IsVerified = member.IsVerified,
//                 CreatedAt = member.CreatedAt,
//                 UpdatedAt = member.UpdatedAt
//             };
//         }

//         public static Member ToEntity(this CreateMemberDto dto, string roleId)
//         {
//             return new Member
//             {
//                 Name = dto.Name,
//                 Email = dto.Email,
//                 Password = dto.Password, // Will be hashed in service layer
//                 Gender = dto.Gender,
//                 Phone = dto.Phone,
//                 DateOfBirth = dto.DateOfBirth,
//                 Address = dto.Address,
//                 Role = roleId,
//                 Status = dto.Status ?? "active",
//                 IsVerified = dto.IsVerified ?? false,
//                 CreatedAt = DateTime.UtcNow,
//                 UpdatedAt = DateTime.UtcNow
//             };
//         }
//     }
// }