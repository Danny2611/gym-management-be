using GymManagement.Application.DTOs.User;
using GymManagement.Application.Interfaces.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// Đăng ký gói tập - Kiểm tra trước khi thanh toán
        /// POST /api/user/packages/register
        /// </summary>
        [HttpPost("packages/register")]
        public async Task<IActionResult> RegisterPackage([FromBody] RegisterPackageRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để đăng ký gói tập"
                    });
                }

                var response = await _membershipService.RegisterPackageAsync(userId, request);

                return Ok(new
                {
                    success = true,
                    message = "Gói tập hợp lệ, sẵn sàng để thanh toán",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách địa điểm tập luyện
        /// GET /api/user/training-locations
        /// </summary>
        [HttpGet("training-locations")]
        public async Task<IActionResult> GetTrainingLocations()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem thông tin địa điểm tập luyện"
                    });
                }

                var locations = await _membershipService.GetMemberTrainingLocationsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = locations.Count,
                    data = locations
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả gói tập đã đăng ký
        /// GET /api/user/my-package
        /// </summary>
        [HttpGet("my-package")]
        public async Task<IActionResult> GetMemberships()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem thông tin gói tập"
                    });
                }

                var memberships = await _membershipService.GetMembershipsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = memberships.Count,
                    data = memberships
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy gói tập đang active
        /// GET /api/user/my-package-active
        /// </summary>
        [HttpGet("my-package-active")]
        public async Task<IActionResult> GetActiveMemberships()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem thông tin gói tập"
                    });
                }

                var memberships = await _membershipService.GetActiveMembershipsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = memberships.Count,
                    data = memberships
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết gói tập theo ID
        /// POST /api/user/my-package/detail
        /// </summary>
        [HttpPost("my-package/detail")]
        public async Task<IActionResult> GetMembershipById([FromBody] MembershipActionRequest request)
        {
            try
            {
                var membership = await _membershipService.GetMembershipByIdAsync(request.MembershipId);

                return Ok(new
                {
                    success = true,
                    data = membership
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Tạm dừng gói tập
        /// PATCH /api/user/my-package/pause
        /// </summary>
        [HttpPatch("my-package/pause")]
        public async Task<IActionResult> PauseMembership([FromBody] MembershipActionRequest request)
        {
            try
            {
                var membership = await _membershipService.PauseMembershipAsync(request.MembershipId);

                return Ok(new
                {
                    success = true,
                    message = "Đã tạm dừng gói tập thành công",
                    data = membership
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Kích hoạt lại gói tập
        /// PATCH /api/user/my-package/resume
        /// </summary>
        [HttpPatch("my-package/resume")]
        public async Task<IActionResult> ResumeMembership([FromBody] MembershipActionRequest request)
        {
            try
            {
                var membership = await _membershipService.ResumeMembershipAsync(request.MembershipId);

                return Ok(new
                {
                    success = true,
                    message = "Đã kích hoạt lại gói tập thành công",
                    data = membership
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết membership (dashboard info)
        /// GET /api/user/my-package/infor-membership
        /// </summary>
        [HttpGet("my-package/infor-membership")]
        public async Task<IActionResult> GetMembershipDetails()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem thông tin hội viên"
                    });
                }

                var details = await _membershipService.GetMembershipDetailsAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = details
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}