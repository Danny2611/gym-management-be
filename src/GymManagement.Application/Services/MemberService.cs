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
        public async Task<bool> UpdateEmailAsync(Guid memberId, string newEmail)
    {
        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.Id == memberId);

        if (member == null)
            return false;

        // Check email duplicate
        var exists = await _context.Members
            .AnyAsync(m => m.Email == newEmail && m.Id != memberId);

        if (exists)
            throw new Exception("Email đã được sử dụng bởi tài khoản khác");

        // Update email
        member.Email = newEmail;
        member.IsVerified = false; // bắt buộc verify lại
        member.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAccountAsync(Guid memberId, string password)
{
    var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId);
    if (member == null)
        return false;

    // Verify password
    var isPasswordValid = member.VerifyPassword(password);
    if (!isPasswordValid)
        throw new Exception("Mật khẩu không chính xác");

    member.Status = "inactive";
    member.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return true;
}

    }
}