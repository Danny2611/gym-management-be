using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminPromotionService
    {
        Task<PromotionListResponseDto> GetAllPromotionsAsync(PromotionQueryOptions options);
        Task<PromotionResponseDto> GetPromotionByIdAsync(string id);
        Task<PromotionResponseDto> CreatePromotionAsync(CreatePromotionDto createDto);
        Task<PromotionResponseDto> UpdatePromotionAsync(string id, UpdatePromotionDto updateDto);
        Task DeletePromotionAsync(string id);
        Task<PromotionEffectivenessDto> GetPromotionEffectivenessAsync(string id);
        Task<List<PromotionResponseDto>> GetActivePromotionsForPackageAsync(string packageId);
        Task<PromotionStatsDto> GetPromotionStatsAsync();
        Task UpdatePromotionStatusesAsync();
    }
}