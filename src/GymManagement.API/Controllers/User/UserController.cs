using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}