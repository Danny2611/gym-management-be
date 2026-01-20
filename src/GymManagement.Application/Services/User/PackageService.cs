using GymManagement.Domain.Entities;
using GymManagement.Application.Interfaces.Repositories.User;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPackageDetailRepository _packageDetailRepository;

    public PackageService(
        IPackageRepository packageRepository,
        IPromotionRepository promotionRepository,
         IPackageDetailRepository packageDetailRepository
         )
    {
        _packageRepository = packageRepository;
        _promotionRepository = promotionRepository;
        _packageDetailRepository = packageDetailRepository;
    }

    public async Task<List<Package>> GetPackagesAsync()
    {
        return await _packageRepository.GetActiveAsync();
    }

    // === getPackageById ===
    public async Task<object?> GetPackageByIdAsync(string id)
    {
        var packageData = await _packageRepository.GetByIdAsync(id);
        if (packageData == null)
            return null;


        var packageDetail = await _packageDetailRepository.GetByPackageIdAsync(id);

        var now = DateTime.Now;

        var promotion = await _promotionRepository
            .GetActivePromotionByPackageIdAsync(id, now);

        decimal? discountedPrice = null;

        if (promotion != null)
        {
            var discountAmount = packageData.Price * (decimal)promotion.Discount / 100;
            discountedPrice = Math.Round(packageData.Price - discountAmount);
        }

        return new
        {
            packageData.Id,
            packageData.Name,
            packageData.Price,
            packageData.Duration,
            packageData.Description,
            packageData.Benefits,
            packageData.Category,
            packageData.Popular,
            packageData.TrainingSessions,
            packageData.SessionDuration,


            Details = packageDetail == null ? null : new
            {
                packageDetail.Schedule,
                packageDetail.TrainingAreas,
                packageDetail.AdditionalServices
            },

            // === promotion ===
            Promotion = promotion == null ? null : new
            {
                promotion.Name,
                promotion.Description,
                promotion.Discount,
                promotion.StartDate,
                promotion.EndDate,
                DiscountedPrice = discountedPrice
            }
        };
    }

}
