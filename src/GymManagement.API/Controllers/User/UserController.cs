using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace GymManagement.API.Controllers.User
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
        /// Lấy danh sách tất cả members
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            try
            {
                var members = await _memberService.GetAllMembersAsync();
                return Ok(new
                {
                    success = true,
                    data = members,
                    message = "Get members successfully"
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
        /// Lấy member theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(string id)
        {
            try
            {
                var member = await _memberService.GetMemberByIdAsync(id);
                return Ok(new
                {
                    success = true,
                    data = member,
                    message = "Get member successfully"
                });
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo member mới (để test)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] Member member)
        {
            try
            {
                var newMember = await _memberService.CreateMemberAsync(member);
                return Ok(new
                {
                    success = true,
                    data = newMember,
                    message = "Create member successfully"
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
    /// Lấy thông tin profile của user hiện tại từ JWT
    /// </summary>

    [HttpGet("current-profile")]
public async Task<IActionResult> GetCurrentProfile()
{
    try
    {
        var userId = User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Không tìm thấy thông tin người dùng"
            });
        }

        var member = await _memberService.GetMemberWithRoleAsync(userId);

        var dto = new MemberProfileDto
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            Phone = member.Phone,
            Address = member.Address,
            BirthDate = member.BirthDate,
            AvatarUrl = member.AvatarUrl,
            RoleName = member.Role?.Name?.ToLower() ?? ""
        };

        return Ok(new { success = true, data = dto });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { success = false, message = ex.Message });
    }
}

[HttpPut("profile")]
public async Task<IActionResult> UpdateProfile([FromBody] MemberUpdateRequest request)
{
    try
    {
        var userId = User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Không tìm thấy thông tin người dùng"
            });
        }

        var updatedMember = await _memberService.UpdateProfileAsync(userId, request);

        return Ok(new
        {
            success = true,
            message = "Cập nhật thông tin thành công",
            data = updatedMember
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            success = false,
            message = ex.Message
        });
    }
}


[Authorize(Roles = "Member")]
[HttpPut("avatar")]
public async Task<IActionResult> UpdateAvatar([FromForm] UploadAvatarDto dto)
{
    if (dto.Avatar == null)
        return BadRequest(new { success = false, message = "Không tìm thấy file avatar" });

    var userId = User.FindFirst("id")?.Value;
    if (userId == null)
        return Unauthorized(new { success = false, message = "Không tìm thấy user" });

    var newAvatarUrl = await _memberService.UpdateAvatarAsync(Guid.Parse(userId), dto.Avatar);

    return Ok(new
    {
        success = true,
        message = "Cập nhật avatar thành công",
        data = new { avatar = newAvatarUrl }
    });
}
[Authorize]
[HttpPut("email")]
public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Email))
        return BadRequest(new
        {
            success = false,
            message = "Email mới không được để trống"
        });

    var memberId = GetUserId(); // Lấy từ token

    try
    {
        var result = await _memberService.UpdateEmailAsync(memberId, request.Email);

        if (!result)
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin hội viên"
            });

        return Ok(new
        {
            success = true,
            message = "Đã cập nhật email, vui lòng xác thực email mới",
            data = new { email = request.Email }
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

[Authorize]
[HttpPost("deactivate")]
public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Password))
    {
        return BadRequest(new
        {
            success = false,
            message = "Vui lòng nhập mật khẩu để xác nhận"
        });
    }

    var memberId = GetUserId();

    try
    {
        var result = await _memberService.DeactivateAccountAsync(memberId, request.Password);

        if (!result)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy thông tin hội viên"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Tài khoản đã bị vô hiệu hóa"
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