using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetAllAsync();
        Task<Member> GetByIdAsync(string id);
        Task<Member> CreateAsync(Member member);
        Task<bool> UpdateAsync(string id, Member member);
        Task<bool> DeleteAsync(string id);
        Task<Member> GetByEmailAsync(string email);
        Task<Member?> GetByIdWithRoleAsync(string id);

    }
}