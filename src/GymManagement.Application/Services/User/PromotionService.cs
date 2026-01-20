using GymManagement.Application.DTOs.User.Promotion;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPackageRepository _packageRepository;

        public PromotionService(
            IPromotionRepository promotionRepository,
            IPackageRepository packageRepository)
        {
            _promotionRepository = promotionRepository;
            _packageRepository = packageRepository;
        }

        /// <summary>
        /// Lấy tất cả promotion đang active (giống getAllActivePromotions của Express)
        /// </summary>
        public async Task<List<PromotionDto>> GetAllActivePromotionsAsync()
        {
            try
            {
                // 1. Lấy danh sách promotions active
                var promotions = await _promotionRepository.GetAllActivePromotionsAsync();

                // 2. Populate package information
                var promotionDtos = new List<PromotionDto>();

                foreach (var promotion in promotions)
                {
                    var promotionDto = await PopulatePromotionAsync(promotion);
                    promotionDtos.Add(promotionDto);
                }

                return promotionDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách khuyến mãi: {ex.Message}");
                throw new Exception("Không thể lấy danh sách khuyến mãi");
            }
        }

        /// <summary>
        /// Lấy tất cả promotions (bao gồm cả inactive)
        /// </summary>
        public async Task<List<PromotionDto>> GetAllPromotionsAsync()
        {
            try
            {
                var promotions = await _promotionRepository.GetAllPromotionsAsync();

                var promotionDtos = new List<PromotionDto>();

                foreach (var promotion in promotions)
                {
                    var promotionDto = await PopulatePromotionAsync(promotion);
                    promotionDtos.Add(promotionDto);
                }

                return promotionDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách khuyến mãi: {ex.Message}");
                throw new Exception("Không thể lấy danh sách khuyến mãi");
            }
        }

        /// <summary>
        /// Lấy promotion theo ID
        /// </summary>
        public async Task<PromotionDto?> GetByIdAsync(string id)
        {
            try
            {
                var promotion = await _promotionRepository.GetByIdAsync(id);

                if (promotion == null)
                {
                    return null;
                }

                return await PopulatePromotionAsync(promotion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin khuyến mãi: {ex.Message}");
                throw new Exception("Không thể lấy thông tin khuyến mãi");
            }
        }

        /// <summary>
        /// Populate package information cho promotion
        /// </summary>
        private async Task<PromotionDto> PopulatePromotionAsync(Promotion promotion)
        {
            var promotionDto = new PromotionDto
            {
                Id = promotion.Id,
                Name = promotion.Name,
                Description = promotion.Description,
                Discount = promotion.Discount,
                Start_date = promotion.StartDate,
                End_date = promotion.EndDate,
                Status = promotion.Status,
                Created_at = promotion.CreatedAt,
                Updated_at = promotion.UpdatedAt
            };

            // Populate applicable packages
            if (promotion.ApplicablePackages != null && promotion.ApplicablePackages.Any())
            {
                var packages = await _packageRepository.GetByIdsAsync(promotion.ApplicablePackages);

                promotionDto.ApplicablePackages = packages.Select(p => new PackageInfoDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Benefits = p.Benefits
                }).ToList();
            }

            return promotionDto;
        }
    }
}