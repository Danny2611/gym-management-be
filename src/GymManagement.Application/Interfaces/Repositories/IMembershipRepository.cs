using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories
{
    public interface IMembershipRepository
    {
        Task<List<Membership>> GetByMemberIdAsync(string memberId);
        Task<List<Membership>> GetActiveMembershipsByMemberIdAsync(string memberId);
        Task<Membership> GetByIdAsync(string id);
        Task<Membership> CreateAsync(Membership membership);
        Task UpdateAsync(string id, Membership membership);
        Task DeleteAsync(string id);
    }
}
