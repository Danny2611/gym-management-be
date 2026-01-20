
using GymManagement.Application.DTOs.User;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface IMembershipService
    {
        Task<RegisterPackageResponse> RegisterPackageAsync(string memberId, RegisterPackageRequest request);
        Task<List<string>> GetMemberTrainingLocationsAsync(string memberId);
        Task<List<MembershipResponse>> GetMembershipsAsync(string memberId);
        Task<List<MembershipResponse>> GetActiveMembershipsAsync(string memberId);
        Task<MembershipResponse> GetMembershipByIdAsync(string membershipId);
        Task<MembershipResponse> PauseMembershipAsync(string membershipId);
        Task<MembershipResponse> ResumeMembershipAsync(string membershipId);
        Task<MembershipDetailsResponse> GetMembershipDetailsAsync(string memberId);
        Task UpdateExpiredMembershipsAsync();
    }
}