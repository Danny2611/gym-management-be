using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.Interfaces.Services.Public;

namespace GymManagement.API.Controllers.Public
{
    [ApiController]
    [Route("api/public")]
    public class BlogCategoryController : ControllerBase
    {
        private readonly IBlogCategoryService _blogCategoryService;
        private readonly ILogger<BlogCategoryController> _logger;

        public BlogCategoryController(
            IBlogCategoryService blogCategoryService,
            ILogger<BlogCategoryController> logger)
        {
            _blogCategoryService = blogCategoryService;
            _logger = logger;
        }

        // GET: api/public/blog-categories
        [HttpGet("blog-categories")]
        public async Task<IActionResult> GetAllBlogCategories()
        {
            try
            {
                var categories = await _blogCategoryService.GetAllCategoriesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách danh mục thành công",
                    data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh mục");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy danh mục"
                });
            }
        }

        // GET: api/public/blog-categories/{slug}
        [HttpGet("blog-categories/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            try
            {
                var category = await _blogCategoryService.GetCategoryBySlugAsync(slug);

                if (category == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy danh mục"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh mục với slug \"{slug}\" thành công",
                    data = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết danh mục");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy chi tiết danh mục"
                });
            }
        }
    }
}