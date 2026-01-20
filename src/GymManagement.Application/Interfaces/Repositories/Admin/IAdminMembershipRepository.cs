
using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminMembershipRepository
    {
        Task<(List<Membership> memberships, int totalCount)> GetAllAsync(MembershipQueryOptions options);
        Task<Membership> GetByIdAsync(string membershipId);
        Task<bool> DeleteAsync(string membershipId);
        Task<MembershipStatsDto> GetStatsAsync();
    }
}
