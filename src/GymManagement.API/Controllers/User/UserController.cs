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


}
    }