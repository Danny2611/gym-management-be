// GymManagement.Application/Interfaces/Services/IMemberService.cs
using GymManagement.Domain.Entities;
using GymManagement.Application.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace GymManagement.Application.Interfaces.Services
{
    public interface IMemberService
    {
        // Basic CRUD
        Task<List<Member>> GetAllMembersAsync();
        Task<Member> GetMemberByIdAsync(string id);
        Task<Member> CreateMemberAsync(Member member);

        // Profile Management
        Task<MemberProfileDto> GetCurrentProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string memberId, MemberUpdateRequest request);
        Task<string> UpdateAvatarAsync(string memberId, IFormFile avatarFile);
        Task<bool> UpdateEmailAsync(string memberId, string newEmail);
        Task<bool> DeactivateAccountAsync(string memberId, string password);
    }
}