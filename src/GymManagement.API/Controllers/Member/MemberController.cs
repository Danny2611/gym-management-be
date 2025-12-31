using GymManagement.Application.DTOs.User;
using GymManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.Member
{
    [ApiController]
    [Route("api/members/profile")] 
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // GET: api/members/profile/current
        [HttpGet("current")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _memberService.GetCurrentProfileAsync(userId);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        // PUT: api/members/profile/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberUpdateRequest request)
        {
            var userId = GetUserId();
            var result = await _memberService.UpdateProfileAsync(userId, request);

            return Ok(new
            {
                success = true,
                data = result,
                message = "Cập nhật thông tin thành công"
            });
        }
        
        private string GetUserId()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Không tìm thấy userId trong token");

            return userId;
        }
    }
}