using GymManagement.Application.DTOs.User;
using GymManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemberEntity = GymManagement.Domain.Entities.Member;

namespace GymManagement.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/members")]
    [Authorize(Roles = "Admin")]
    public class AdminMemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public AdminMemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemberEntity request)
        {
            var result = await _memberService.CreateMemberAsync(request);
            return Ok(new { success = true, data = result, message = "Tạo member thành công" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _memberService.GetAllMembersAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _memberService.GetMemberByIdAsync(id);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MemberUpdateRequest request)
        {
            var result = await _memberService.UpdateProfileAsync(id, request);
            return Ok(new { success = true, data = result, message = "Cập nhật member thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _memberService.DeleteMemberAsync(id);
            if (!result)
            {
                return NotFound(new { success = false, message = "Không tìm thấy member" });
            }
            return Ok(new { success = true, message = "Xóa member thành công" });
        }
    }


    public class DeleteMemberRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}