using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IMemberService
    {
        Task<List<Member>> GetAllMembersAsync();
        Task<Member> GetMemberByIdAsync(string id);
        Task<Member> CreateMemberAsync(Member member);
        Task<Member> UpdateProfileAsync(string userId, MemberUpdateRequest request);
        Task<bool> UpdateEmailAsync(Guid memberId, string newEmail);
          Task<bool> DeactivateAccountAsync(Guid memberId, string password);
        


    }
}