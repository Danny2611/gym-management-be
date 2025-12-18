using GymManagement.Domain.Entities;

public interface IPromotionRepository
{
    Task<List<Promotion>> GetActiveAsync();
    Task<Promotion?> GetActivePromotionForPackageAsync(Guid packageId, DateTime now);
}
