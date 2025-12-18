using GymManagement.Domain.Entities;

public interface IPromotionService
{
    Task<List<Promotion>> GetActivePromotionsAsync();
}
