using GymManagement.Domain.Entities;
using GymManagement.Application.Interfaces.Repositories;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IPromotionRepository _promotionRepository;

    public PackageService(
        IPackageRepository packageRepository,
        IPromotionRepository promotionRepository)
    {
        _packageRepository = packageRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<List<Package>> GetPackagesAsync()
    {
        return await _packageRepository.GetActiveAsync();
    }

    public async Task<object?> GetPackageByIdAsync(Guid id)
    {
        var package = await _packageRepository.GetByIdAsync(id);
        if (package == null) return null;

        var detail = await _packageRepository.GetDetailByPackageIdAsync(id);

        var now = DateTime.UtcNow;
        var promotion = await _promotionRepository
            .GetActivePromotionForPackageAsync(id, now);

        decimal? discountedPrice = null;

        if (promotion != null)
        {
            var discountAmount = (package.Price * (decimal)promotion.Discount) / 100;
            discountedPrice = Math.Round(package.Price - discountAmount);
        }

        return new
        {
            package,
            details = detail,
            promotion = promotion == null ? null : new
            {
                promotion.Name,
                promotion.Description,
                promotion.Discount,
                promotion.StartDate,
                promotion.EndDate,
                discountedPrice
            }
        };
    }
}
