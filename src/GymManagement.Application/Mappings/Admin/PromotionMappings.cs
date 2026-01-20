using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.Admin
{
    public static class AdminPromotionMapping
    {
        public static PromotionResponseDto ToDto(
            this Promotion promotion,
            List<Package> packages)
        {
            return new PromotionResponseDto
            {
                Id = promotion.Id,
                Name = promotion.Name,
                Description = promotion.Description,
                Discount = promotion.Discount,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Status = promotion.Status,
                ApplicablePackages = packages.Select(p => new PromotionPackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList(),
                CreatedAt = promotion.CreatedAt,
                UpdatedAt = promotion.UpdatedAt
            };
        }

        public static Promotion ToEntity(this CreatePromotionDto dto)
        {
            return new Promotion
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Discount = dto.Discount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ApplicablePackages = dto.ApplicablePackages,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}