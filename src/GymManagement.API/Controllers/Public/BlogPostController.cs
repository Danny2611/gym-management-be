using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.Interfaces.Services.Public;

namespace GymManagement.API.Controllers.Public
{
    [ApiController]
    [Route("api/public")]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(
            IBlogPostService blogPostService,
            ILogger<BlogPostController> logger)
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        // GET: api/public/blogs
        [HttpGet("blogs")]
        public async Task<IActionResult> GetAllPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _blogPostService.GetAllPostsAsync(page, pageSize);

                return Ok(new
                {
                    success = true,
                    message = "Lấy tất cả bài viết thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả bài viết");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy tất cả bài viết"
                });
            }
        }

        // GET: api/public/blog/latest
        [HttpGet("blog/latest")]
        public async Task<IActionResult> GetLatestPosts([FromQuery] int limit = 5)
        {
            try
            {
                var posts = await _blogPostService.GetLatestPostsAsync(limit);

                return Ok(new
                {
                    success = true,
                    message = "Lấy bài viết mới nhất thành công",
                    data = posts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bài viết mới nhất");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy bài viết mới nhất"
                });
            }
        }

        // GET: api/public/blog/featured
        [HttpGet("blog/featured")]
        public async Task<IActionResult> GetFeaturedPosts([FromQuery] int limit = 5)
        {
            try
            {
                var posts = await _blogPostService.GetFeaturedPostsAsync(limit);

                return Ok(new
                {
                    success = true,
                    message = "Lấy bài viết nổi bật thành công",
                    data = posts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bài viết nổi bật");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy bài viết nổi bật"
                });
            }
        }

        // GET: api/public/blog/tag/{tag}
        [HttpGet("blog/tag/{tag}")]
        public async Task<IActionResult> GetPostsByTag(
            string tag,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _blogPostService.GetPostsByTagAsync(tag, page, pageSize);

                return Ok(new
                {
                    success = true,
                    message = $"Lấy bài viết theo thẻ \"{tag}\" thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bài viết theo thẻ");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy bài viết theo thẻ"
                });
            }
        }

        // GET: api/public/blog/category/{slug}
        [HttpGet("blog/category/{slug}")]
        public async Task<IActionResult> GetPostsByCategory(
            string slug,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _blogPostService.GetPostsByCategoryAsync(slug, page, pageSize);

                return Ok(new
                {
                    success = true,
                    message = $"Lấy bài viết theo danh mục \"{slug}\" thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy bài viết theo danh mục");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy bài viết theo danh mục"
                });
            }
        }

        // GET: api/public/blog/{slug}
        [HttpGet("blog/{slug}")]
        public async Task<IActionResult> GetPostBySlug(string slug)
        {
            try
            {
                var post = await _blogPostService.GetPostBySlugAsync(slug);

                if (post == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy bài viết"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Lấy bài viết với slug \"{slug}\" thành công",
                    data = post
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết bài viết");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message ?? "Lỗi server khi lấy chi tiết bài viết"
                });
            }
        }
    }



}