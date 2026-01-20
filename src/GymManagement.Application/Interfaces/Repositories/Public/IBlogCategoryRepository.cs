using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Public
{
    public interface IBlogCategoryRepository
    {
        Task<List<BlogCategory>> GetAllCategoriesAsync();
        Task<BlogCategory> GetCategoryBySlugAsync(string slug);
        Task<BlogCategory> GetByIdAsync(string id);
    }
}
