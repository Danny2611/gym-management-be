// // GymManagement.Application/Interfaces/Repositories/IMemberRepository.cs
// using GymManagement.Domain.Entities;

// namespace GymManagement.Application.Interfaces.Repositories
// {
//     public interface IMemberRepository
//     {
//         Task<List<Member>> GetAllAsync();
//         Task<Member> GetByIdAsync(string id);
//         Task<Member> GetByEmailAsync(string email);
//         Task<Member> GetByPhoneAsync(string phone);
//         Task<Member> CreateAsync(Member member);
//         Task UpdateAsync(string id, Member member);
//         Task DeleteAsync(string id);
//     }
// }