using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminMembershipService
    {
        Task<MembershipListResponseDto> GetAllMembershipsAsync(MembershipQueryOptions options);
        Task<MembershipResponseDto> GetMembershipByIdAsync(string membershipId);
        Task<bool> DeleteMembershipAsync(string membershipId);
        Task<MembershipStatsDto> GetMembershipStatsAsync();
    }
}