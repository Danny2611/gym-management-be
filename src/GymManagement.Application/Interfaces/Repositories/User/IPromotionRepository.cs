using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IPromotionRepository
    {
        Task<List<Promotion>> GetAllActivePromotionsAsync();
        Task<List<Promotion>> GetAllPromotionsAsync();
        Task<Promotion?> GetByIdAsync(string id);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task UpdateAsync(string id, Promotion promotion);
        Task DeleteAsync(string id);
        Task<Promotion?> GetActivePromotionByPackageIdAsync(string packageId, DateTime now);
    }
}