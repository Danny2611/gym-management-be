using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.Admin
{
    public class AdminPromotionService : IAdminPromotionService
    {
        private readonly IAdminPromotionRepository _promotionRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IAdminMembershipRepository _membershipRepository;

        public AdminPromotionService(
            IAdminPromotionRepository promotionRepository,
            IPackageRepository packageRepository,
            IAdminMembershipRepository membershipRepository)
        {
            _promotionRepository = promotionRepository;
            _packageRepository = packageRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<PromotionListResponseDto> GetAllPromotionsAsync(
            PromotionQueryOptions options)
        {
            var (promotions, totalCount) = await _promotionRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            var promotionDtos = new List<PromotionResponseDto>();

            foreach (var promotion in promotions)
            {
                var packages = new List<Package>();
                foreach (var packageId in promotion.ApplicablePackages)
                {
                    var package = await _packageRepository.GetByIdAsync(packageId);
                    if (package != null)
                    {
                        packages.Add(package);
                    }
                }

                promotionDtos.Add(promotion.ToDto(packages));
            }

            return new PromotionListResponseDto
            {
                Promotions = promotionDtos,
                TotalPromotions = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<PromotionResponseDto> GetPromotionByIdAsync(string id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);

            if (promotion == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình khuyến mãi");
            }

            var packages = new List<Package>();
            foreach (var packageId in promotion.ApplicablePackages)
            {
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package != null)
                {
                    packages.Add(package);
                }
            }

            return promotion.ToDto(packages);
        }

        public async Task<PromotionResponseDto> CreatePromotionAsync(CreatePromotionDto createDto)
        {
            // Validate dates
            if (createDto.EndDate <= createDto.StartDate)
            {
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu");
            }

            // Validate discount
            if (createDto.Discount <= 0 || createDto.Discount > 100)
            {
                throw new ArgumentException("Phần trăm giảm giá phải từ 1% đến 100%");
            }

            // Validate applicable packages exist
            var packageCount = 0;
            foreach (var packageId in createDto.ApplicablePackages)
            {
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package != null)
                {
                    packageCount++;
                }
            }

            if (packageCount != createDto.ApplicablePackages.Count)
            {
                throw new ArgumentException("Một hoặc nhiều gói không tồn tại");
            }

            var promotion = createDto.ToEntity();
            var createdPromotion = await _promotionRepository.CreateAsync(promotion);

            return await GetPromotionByIdAsync(createdPromotion.Id);
        }

        public async Task<PromotionResponseDto> UpdatePromotionAsync(
            string id,
            UpdatePromotionDto updateDto)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);

            if (promotion == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình khuyến mãi");
            }

            // Validate dates if provided
            var startDate = updateDto.StartDate ?? promotion.StartDate;
            var endDate = updateDto.EndDate ?? promotion.EndDate;

            if (endDate <= startDate)
            {
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu");
            }

            // Validate discount if provided
            if (updateDto.Discount.HasValue)
            {
                if (updateDto.Discount.Value <= 0 || updateDto.Discount.Value > 100)
                {
                    throw new ArgumentException("Phần trăm giảm giá phải từ 1% đến 100%");
                }
            }

            // Validate applicable packages if provided
            if (updateDto.ApplicablePackages != null && updateDto.ApplicablePackages.Any())
            {
                var packageCount = 0;
                foreach (var packageId in updateDto.ApplicablePackages)
                {
                    var package = await _packageRepository.GetByIdAsync(packageId);
                    if (package != null)
                    {
                        packageCount++;
                    }
                }

                if (packageCount != updateDto.ApplicablePackages.Count)
                {
                    throw new ArgumentException("Một hoặc nhiều gói không tồn tại");
                }
            }

            var updatedPromotion = await _promotionRepository.UpdateAsync(id, updateDto);

            if (updatedPromotion == null)
            {
                throw new InvalidOperationException("Cập nhật thất bại");
            }

            return await GetPromotionByIdAsync(updatedPromotion.Id);
        }

        public async Task DeletePromotionAsync(string id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);

            if (promotion == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình khuyến mãi");
            }

            // Check if promotion is currently active
            var now = DateTime.UtcNow;
            var isCurrentlyActive = promotion.Status == "active" &&
                promotion.StartDate <= now &&
                promotion.EndDate >= now;

            if (isCurrentlyActive)
            {
                throw new InvalidOperationException("Không thể xóa chương trình khuyến mãi đang hoạt động");
            }

            var deleted = await _promotionRepository.DeleteAsync(id);

            if (!deleted)
            {
                throw new InvalidOperationException("Xóa thất bại");
            }
        }

        public async Task<PromotionEffectivenessDto> GetPromotionEffectivenessAsync(string id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);

            if (promotion == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình khuyến mãi");
            }

            var packageStats = new List<PromotionPackageStatsDto>();
            var totalMemberships = 0;
            var totalRevenue = 0m;

            // Analyze each applicable package
            foreach (var packageId in promotion.ApplicablePackages)
            {
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package == null) continue;

                // Get memberships created during promotion period for this package
                var membershipOptions = new MembershipQueryOptions
                {
                    PackageId = packageId,
                    Page = 1,
                    Limit = int.MaxValue // Get all
                };

                var (memberships, _) = await _membershipRepository.GetAllAsync(membershipOptions);

                // Filter by promotion period
                var promotionMemberships = memberships
                    .Where(m => m.CreatedAt >= promotion.StartDate && m.CreatedAt <= promotion.EndDate)
                    .ToList();

                var packageMemberships = promotionMemberships.Count;
                var packageRevenue = packageMemberships * package.Price * (1 - promotion.Discount / 100);

                // Calculate conversion rate
                var totalPackageMemberships = memberships.Count;
                var conversionRate = totalPackageMemberships > 0
                    ? Math.Round((decimal)packageMemberships / totalPackageMemberships * 100, 2)
                    : 0;

                packageStats.Add(new PromotionPackageStatsDto
                {
                    PackageId = packageId,
                    PackageName = package.Name,
                    TotalMemberships = packageMemberships,
                    TotalRevenue = Math.Round(packageRevenue, 2),
                    ConversionRate = conversionRate
                });

                totalMemberships += packageMemberships;
                totalRevenue += packageRevenue;
            }

            var averageConversionRate = packageStats.Any()
                ? Math.Round(packageStats.Average(s => s.ConversionRate), 2)
                : 0;

            return new PromotionEffectivenessDto
            {
                PromotionId = promotion.Id,
                PromotionName = promotion.Name,
                PromotionPeriod = new PromotionPeriodDto
                {
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate
                },
                PackageStats = packageStats,
                TotalMemberships = totalMemberships,
                TotalRevenue = Math.Round(totalRevenue, 2),
                AverageConversionRate = averageConversionRate
            };
        }

        public async Task<List<PromotionResponseDto>> GetActivePromotionsForPackageAsync(string packageId)
        {
            var promotions = await _promotionRepository.GetActivePromotionsForPackageAsync(packageId);

            var promotionDtos = new List<PromotionResponseDto>();

            foreach (var promotion in promotions)
            {
                var packages = new List<Package>();
                foreach (var pkgId in promotion.ApplicablePackages)
                {
                    var package = await _packageRepository.GetByIdAsync(pkgId);
                    if (package != null)
                    {
                        packages.Add(package);
                    }
                }

                promotionDtos.Add(promotion.ToDto(packages));
            }

            return promotionDtos;
        }

        public async Task<PromotionStatsDto> GetPromotionStatsAsync()
        {
            return await _promotionRepository.GetStatsAsync();
        }

        public async Task UpdatePromotionStatusesAsync()
        {
            await _promotionRepository.UpdatePromotionStatusesAsync();
        }
    }
}