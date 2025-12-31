using GymManagement.Application.DTOs.User;
using GymManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Tạo member mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMemberRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var result = await _memberService.CreateMemberAsync(request);
                
                return Ok(new 
                { 
                    success = true, 
                    data = result, 
                    message = "Tạo member thành công" 
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
        /// Lấy danh sách tất cả members
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _memberService.GetAllMembersAsync();
                
                return Ok(new 
                { 
                    success = true, 
                    data = result 
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

        /// <summary>
        /// Lấy thông tin member theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _memberService.GetMemberByIdAsync(id);
                
                return Ok(new 
                { 
                    success = true, 
                    data = result 
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
        /// Cập nhật thông tin member
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MemberUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var result = await _memberService.UpdateProfileAsync(id, request);
                
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy member"
                    });
                }

                // Get updated member
                var updatedMember = await _memberService.GetMemberByIdAsync(id);
                
                return Ok(new 
                { 
                    success = true, 
                    data = updatedMember, 
                    message = "Cập nhật member thành công" 
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
        /// Xóa member
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _memberService.DeleteMemberAsync(id);
                
                if (!result)
                {
                    return NotFound(new 
                    { 
                        success = false, 
                        message = "Không tìm thấy member" 
                    });
                }
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Xóa member thành công" 
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