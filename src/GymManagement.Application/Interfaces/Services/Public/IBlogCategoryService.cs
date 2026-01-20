using GymManagement.Application.DTOs.Public;

namespace GymManagement.Application.Interfaces.Services.Public
{
    public interface IBlogCategoryService
    {
        Task<List<BlogCategoryResponseDto>> GetAllCategoriesAsync();
        Task<BlogCategoryResponseDto> GetCategoryBySlugAsync(string slug);
    }
}