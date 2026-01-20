using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IMembershipRepository
    {
        Task<Membership?> GetByIdAsync(string id);
        Task<Membership?> GetByMemberAndPackageAsync(string memberId, string packageId, string status);
        Task<List<Membership>> GetByMemberIdAsync(string memberId);
        Task<List<Membership>> GetAllAsync();
        Task<Membership> CreateAsync(Membership membership);
        Task UpdateAsync(string id, Membership membership);
        Task DeleteManyAsync(string memberId, string packageId, List<string> statuses);
    }
}
