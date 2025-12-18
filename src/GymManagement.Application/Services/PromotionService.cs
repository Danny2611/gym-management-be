using GymManagement.Domain.Entities;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<List<Promotion>> GetActivePromotionsAsync()
    {
        return await _promotionRepository.GetActiveAsync();
    }
}
