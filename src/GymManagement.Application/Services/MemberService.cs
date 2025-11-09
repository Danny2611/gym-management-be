using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllAsync();
        }

        public async Task<Member> GetMemberByIdAsync(string id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                throw new Exception("Member not found");
            }
            return member;
        }

        public async Task<Member> CreateMemberAsync(Member member)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingMember = await _memberRepository.GetByEmailAsync(member.Email);
            if (existingMember != null)
            {
                throw new Exception("Email already exists");
            }

            return await _memberRepository.CreateAsync(member);
        }
    }
}