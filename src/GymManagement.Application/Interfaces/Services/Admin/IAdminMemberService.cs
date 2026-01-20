using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminMemberService
    {
        Task<MemberListResponseDto> GetAllMembersAsync(MemberQueryOptions options);
        Task<MemberResponseDto> GetMemberByIdAsync(string memberId);
        Task<MemberResponseDto> CreateMemberAsync(CreateMemberDto dto);
        Task<MemberResponseDto> UpdateMemberAsync(string memberId, UpdateMemberDto dto);
        Task<MemberResponseDto> UpdateMemberStatusAsync(string memberId, string status);
        Task<bool> DeleteMemberAsync(string memberId);
        Task<MemberStatsDto> GetMemberStatsAsync();
    }
}





