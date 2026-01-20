using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminPromotionRepository
    {
        Task<(List<Promotion> promotions, int totalCount)> GetAllAsync(PromotionQueryOptions options);
        Task<Promotion?> GetByIdAsync(string id);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> UpdateAsync(string id, UpdatePromotionDto updateData);
        Task<bool> DeleteAsync(string id);
        Task<List<Promotion>> GetActivePromotionsForPackageAsync(string packageId);
        Task<PromotionStatsDto> GetStatsAsync();
        Task UpdatePromotionStatusesAsync();
    }
}