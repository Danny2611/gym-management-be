using GymManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.Member
{
    [ApiController]
    [Route("api/member")]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Lấy danh sách địa điểm tập luyện theo gói đã đăng ký
        /// </summary>
        [HttpGet("training-locations")]
        public IActionResult GetTrainingLocations()
        {
            return Ok(new
            {
                success = true,
                message = "API training-locations đã được định nghĩa"
            });
        }

        /// <summary>
        /// Lấy danh sách gói tập đã đăng ký
        /// </summary>
        [HttpGet("my-package")]
        public IActionResult GetMyPackages()
        {
            return Ok(new
            {
                success = true,
                message = "API my-package đã được định nghĩa"
            });
        }

        /// <summary>
        /// Lấy danh sách gói tập đang hoạt động
        /// </summary>
        [HttpGet("my-package-active")]
        public IActionResult GetMyActivePackages()
        {
            return Ok(new
            {
                success = true,
                message = "API my-package-active đã được định nghĩa"
            });
        }

        /// <summary>
        /// Lấy chi tiết gói tập đã đăng ký
        /// </summary>
        [HttpGet("my-package/detail")]
        public IActionResult GetMyPackageDetail([FromQuery] Guid membershipId)
        {
            return Ok(new
            {
                success = true,
                message = "API my-package/detail đã được định nghĩa",
                membershipId
            });
        }

        /// <summary>
        /// Tạm dừng gói tập
        /// </summary>
        [HttpPatch("my-package/pause")]
        public IActionResult PausePackage([FromQuery] Guid membershipId)
        {
            return Ok(new
            {
                success = true,
                message = "API pause package đã được định nghĩa",
                membershipId
            });
        }

        /// <summary>
        /// Kích hoạt lại gói tập
        /// </summary>
        [HttpPatch("my-package/resume")]
        public IActionResult ResumePackage([FromQuery] Guid membershipId)
        {
            return Ok(new
            {
                success = true,
                message = "API resume package đã được định nghĩa",
                membershipId
            });
        }
        /// <summary>
        /// Đăng ký gói tập
        /// </summary>
        [HttpPost("packages/register")]
        public IActionResult RegisterPackage()
        {
            return Ok(new
            {
                success = true,
                message = "API register package đã được định nghĩa"
            });
        }

        /// <summary>
        /// Lấy thông tin membership của hội viên
        /// </summary>
        [HttpGet("my-package/infor-membership")]
        public IActionResult GetMembershipInfo()
        {
            return Ok(new
            {
                success = true,
                message = "API infor-membership đã được định nghĩa"
            });
        }

    }
}

