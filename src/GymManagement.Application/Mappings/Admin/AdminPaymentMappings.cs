using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;
using MongoDB.Bson;

namespace GymManagement.Application.Mappings.Admin
{
    public static class PaymentMapping
    {
        public static PaymentResponseDto ToDto(
            this Payment payment,
            Member member,
            Package package)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                Member = new PaymentMemberDto
                {
                    Id = member.Id,
                    Name = member.Name,
                    Email = member.Email,
                    Avatar = member.Avatar
                },
                Package = new PaymentPackageDto
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    TrainingSessions = package.TrainingSessions
                },
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                PaymentInfo = payment.PaymentInfo != null
                    ? BsonTypeMapper.MapToDotNetValue(payment.PaymentInfo)
                    : null,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }
    }
}