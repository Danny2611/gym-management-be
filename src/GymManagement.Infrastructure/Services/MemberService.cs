using GymManagement.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace GymManagement.Infrastructure.Services
{
    public class MemberService : IMemberService
    {

public async Task<Member> GetMemberWithRoleAsync(string id)
{
    var member = await _memberRepository.GetByIdWithRoleAsync(id);
    if (member == null)
        throw new Exception("Member not found");

    return member;
}
        private readonly IMemberRepository _memberRepository;
        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }



        public async Task<Member> UpdateProfileAsync(string userId, MemberUpdateRequest request)
{
    var member = await _memberRepository.GetByIdAsync(userId);

    if (member == null)
    {
        throw new Exception("Member not found");
    }

    bool hasChanges = false;

    if (request.FullName != null && request.FullName != member.FullName)
    {
        member.FullName = request.FullName;
        hasChanges = true;
    }

    if (request.Gender != null && request.Gender != member.Gender)
    {
        member.Gender = request.Gender;
        hasChanges = true;
    }

    if (request.Phone != null && request.Phone != member.Phone)
    {
        member.Phone = request.Phone;
        hasChanges = true;
    }

    if (request.BirthDate.HasValue && request.BirthDate != member.BirthDate)
    {
        member.BirthDate = request.BirthDate;
        hasChanges = true;
    }

    if (request.Address != null && request.Address != member.Address)
    {
        member.Address = request.Address;
        hasChanges = true;
    }

    if (request.Email != null && request.Email != member.Email)
    {
        member.Email = request.Email;
        hasChanges = true;
    }

    if (!hasChanges)
    {
        return member; // Không thay đổi gì
    }

    member.UpdatedAt = DateTime.UtcNow;

    await _memberRepository.UpdateAsync(member);
    return member;
}

    }
    
}