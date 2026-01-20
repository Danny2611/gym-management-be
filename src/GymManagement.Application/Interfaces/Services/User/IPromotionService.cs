using GymManagement.Application.DTOs.User.Promotion;
using GymManagement.Domain.Entities;

public interface IPromotionService
{
    Task<List<PromotionDto>> GetAllActivePromotionsAsync();
    Task<List<PromotionDto>> GetAllPromotionsAsync();
    Task<PromotionDto?> GetByIdAsync(string id);

}
