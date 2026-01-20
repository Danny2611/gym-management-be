
using GymManagement.Domain.Entities;

using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminMemberRepository
    {
        Task<(List<Member> members, int totalCount)> GetAllAsync(MemberQueryOptions options);
        Task<Member> GetByIdAsync(string memberId);
        Task<Member> GetByEmailAsync(string email);
        Task<Member> CreateAsync(Member member);
        Task<Member> UpdateAsync(string memberId, Member member);
        Task<Member> UpdateStatusAsync(string memberId, string status);
        Task<bool> DeleteAsync(string memberId);
        Task<MemberStatsDto> GetStatsAsync();
    }
}