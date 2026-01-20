using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/members")]
    public class MemberController : ControllerBase
    {
        private readonly IAdminMemberService _memberService;
        private readonly ILogger<MemberController> _logger;

        public MemberController(
            IAdminMemberService memberService,
            ILogger<MemberController> logger)
        {
            _memberService = memberService;
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

        [HttpGet("stats")]
        public async Task<IActionResult> GetMemberStats()
        {
            try
            {
                var stats = await _memberService.GetMemberStatsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê hội viên thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê hội viên"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] CreateMemberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                if (string.IsNullOrEmpty(dto.Name) ||
                    string.IsNullOrEmpty(dto.Email) ||
                    string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu thông tin bắt buộc (tên, email, mật khẩu)"
                    });
                }

                var member = await _memberService.CreateMemberAsync(dto);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Tạo hội viên mới thành công",
                    data = member
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
                _logger.LogError(ex, "Lỗi khi tạo hội viên mới");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string search = null,
            [FromQuery] string status = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = "asc")
        {
            try
            {
                var userRole = GetUserRole();
                _logger.LogInformation("User role: {Role}", userRole);

                var options = new MemberQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var result = await _memberService.GetAllMembersAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách hội viên thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách hội viên"
                });
            }
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberById(string memberId)
        {
            try
            {
                if (string.IsNullOrEmpty(memberId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu ID hội viên"
                    });
                }

                var member = await _memberService.GetMemberByIdAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin hội viên thành công",
                    data = member
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{memberId}")]
        public async Task<IActionResult> UpdateMember(
            string memberId,
            [FromBody] UpdateMemberDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(memberId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu ID hội viên"
                    });
                }

                var member = await _memberService.UpdateMemberAsync(memberId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật hội viên thành công",
                    data = member
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
                _logger.LogError(ex, "Lỗi khi cập nhật hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPatch("{memberId}/status")]
        public async Task<IActionResult> UpdateMemberStatus(
            string memberId,
            [FromBody] UpdateMemberStatusDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(dto.Status))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu ID hội viên hoặc trạng thái"
                    });
                }

                var member = await _memberService.UpdateMemberStatusAsync(memberId, dto.Status);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trạng thái hội viên thành công",
                    data = member
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
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(string memberId)
        {
            try
            {
                if (string.IsNullOrEmpty(memberId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu ID hội viên"
                    });
                }

                await _memberService.DeleteMemberAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Xóa hội viên thành công"
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa hội viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}