using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.Interfaces.Services.Admin;
using System.Security.Claims;
using GymManagement.Application.DTOs.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminMembershipController : ControllerBase
    {
        private readonly IAdminMembershipService _membershipService;
        private readonly ILogger<AdminMembershipController> _logger;

        public AdminMembershipController(
            IAdminMembershipService membershipService,
            ILogger<AdminMembershipController> logger)
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        [HttpGet("memberships/stats")]
        public async Task<IActionResult> GetMembershipStats()
        {
            try
            {
                var stats = await _membershipService.GetMembershipStatsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê đăng ký thành viên thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê đăng ký thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê đăng ký thành viên"
                });
            }
        }

        [HttpGet("memberships")]
        public async Task<IActionResult> GetAllMemberships(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string search = null,
            [FromQuery] string status = null,
            [FromQuery] string memberId = null,
            [FromQuery] string packageId = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = "asc")
        {
            try
            {
                var options = new MembershipQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    MemberId = memberId,
                    PackageId = packageId,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var result = await _membershipService.GetAllMembershipsAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đăng ký thành viên thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đăng ký thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách đăng ký thành viên"
                });
            }
        }

        [HttpGet("membership/{membershipId}")]
        public async Task<IActionResult> GetMembershipById(string membershipId)
        {
            try
            {
                if (string.IsNullOrEmpty(membershipId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID đăng ký thành viên không hợp lệ"
                    });
                }

                var membership = await _membershipService.GetMembershipByIdAsync(membershipId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin đăng ký thành viên thành công",
                    data = membership
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin đăng ký thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thông tin đăng ký thành viên"
                });
            }
        }

        [HttpDelete("membership/delete")]
        public async Task<IActionResult> DeleteMembership([FromBody] DeleteMembershipDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto?.Id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID đăng ký thành viên không được để trống"
                    });
                }

                await _membershipService.DeleteMembershipAsync(dto.Id);

                return Ok(new
                {
                    success = true,
                    message = "Xóa đăng ký thành viên thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đăng ký thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xóa đăng ký thành viên"
                });
            }
        }
    }
}