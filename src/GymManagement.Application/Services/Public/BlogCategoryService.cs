using GymManagement.Application.DTOs.Public;
using GymManagement.Application.Interfaces.Repositories.Public;
using GymManagement.Application.Interfaces.Services.Public;
using GymManagement.Application.Mappings.Public;

namespace GymManagement.Application.Services.Public
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly IBlogCategoryRepository _blogCategoryRepository;

        public BlogCategoryService(IBlogCategoryRepository blogCategoryRepository)
        {
            _blogCategoryRepository = blogCategoryRepository;
        }

        public async Task<List<BlogCategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _blogCategoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => c.ToDto()).ToList();
        }

        public async Task<BlogCategoryResponseDto> GetCategoryBySlugAsync(string slug)
        {
            var category = await _blogCategoryRepository.GetCategoryBySlugAsync(slug);
            return category?.ToDto();
        }
    }
}