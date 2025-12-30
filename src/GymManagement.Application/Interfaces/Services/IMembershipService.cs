using GymManagement.Application.DTOs.User.Responses;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IMembershipService
    {
        Task<List<string>> GetMemberTrainingLocationsAsync(string memberId);
        Task<List<MembershipResponse>> GetMemberMembershipsAsync(string memberId);
    }
}
