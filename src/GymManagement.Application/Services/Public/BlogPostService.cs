using GymManagement.Application.DTOs.Public;
using GymManagement.Application.Interfaces.Repositories.Public;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.Public;
using GymManagement.Application.Mappings.Public;

namespace GymManagement.Application.Services.Public
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogCategoryRepository _blogCategoryRepository;
        private readonly IMemberRepository _memberRepository;

        public BlogPostService(
            IBlogPostRepository blogPostRepository,
            IBlogCategoryRepository blogCategoryRepository,
            IMemberRepository memberRepository)
        {
            _blogPostRepository = blogPostRepository;
            _blogCategoryRepository = blogCategoryRepository;
            _memberRepository = memberRepository;
        }

        public async Task<List<BlogPostResponseDto>> GetLatestPostsAsync(int limit)
        {
            var posts = await _blogPostRepository.GetLatestPostsAsync(limit);
            return await PopulatePostsAsync(posts);
        }

        public async Task<List<BlogPostResponseDto>> GetFeaturedPostsAsync(int limit)
        {
            var posts = await _blogPostRepository.GetFeaturedPostsAsync(limit);
            return await PopulatePostsAsync(posts);
        }

        public async Task<BlogPostListResponseDto> GetAllPostsAsync(int page, int pageSize)
        {
            var (posts, total) = await _blogPostRepository.GetAllPostsAsync(page, pageSize);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            var populatedPosts = await PopulatePostsAsync(posts);

            return new BlogPostListResponseDto
            {
                Posts = populatedPosts,
                Total = total,
                TotalPages = totalPages
            };
        }

        public async Task<BlogPostListResponseDto> GetPostsByCategoryAsync(
            string categorySlug, int page, int pageSize)
        {
            var category = await _blogCategoryRepository.GetCategoryBySlugAsync(categorySlug);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            var (posts, total) = await _blogPostRepository.GetPostsByCategoryAsync(
                category.Id, page, pageSize);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            var populatedPosts = await PopulatePostsAsync(posts);

            return new BlogPostListResponseDto
            {
                Posts = populatedPosts,
                Total = total,
                TotalPages = totalPages
            };
        }

        public async Task<BlogPostListResponseDto> GetPostsByTagAsync(
            string tag, int page, int pageSize)
        {
            var (posts, total) = await _blogPostRepository.GetPostsByTagAsync(tag, page, pageSize);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            var populatedPosts = await PopulatePostsAsync(posts);

            return new BlogPostListResponseDto
            {
                Posts = populatedPosts,
                Total = total,
                TotalPages = totalPages
            };
        }

        public async Task<BlogPostResponseDto> GetPostBySlugAsync(string slug)
        {
            var post = await _blogPostRepository.GetPostBySlugAsync(slug);

            if (post == null)
            {
                return null;
            }

            var author = await _memberRepository.GetByIdAsync(post.AuthorId);
            var category = await _blogCategoryRepository.GetByIdAsync(post.CategoryId);

            return post.ToDto(author, category);
        }

        private async Task<List<BlogPostResponseDto>> PopulatePostsAsync(List<Domain.Entities.BlogPost> posts)
        {
            var result = new List<BlogPostResponseDto>();

            foreach (var post in posts)
            {
                var author = await _memberRepository.GetByIdAsync(post.AuthorId);
                var category = await _blogCategoryRepository.GetByIdAsync(post.CategoryId);
                result.Add(post.ToDto(author, category));
            }

            return result;
        }
    }


}