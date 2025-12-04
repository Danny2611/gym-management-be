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
    }
    
}