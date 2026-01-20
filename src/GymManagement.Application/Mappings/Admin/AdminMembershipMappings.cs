
using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.Admin
{
    public static class AdminMembershipMappings
    {
        public static MembershipResponseDto ToDto(
            this Membership membership,
            Member member,
            Package package)
        {
            return new MembershipResponseDto
            {
                Id = membership.Id,
                Member = new MembershipMemberDto
                {
                    Id = member.Id,
                    Name = member.Name,
                    Email = member.Email,
                    Avatar = member.Avatar
                },
                Package = new MembershipPackageDto
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    TrainingSessions = package.TrainingSessions
                },
                PaymentId = membership.PaymentId,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                AutoRenew = membership.AutoRenew,
                Status = membership.Status,
                AvailableSessions = membership.AvailableSessions,
                UsedSessions = membership.UsedSessions,
                LastSessionsReset = membership.LastSessionsReset,
                CreatedAt = membership.CreatedAt,
                UpdatedAt = membership.UpdatedAt
            };
        }
    }
}