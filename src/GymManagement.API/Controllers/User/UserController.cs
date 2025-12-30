// GymManagement.API/Controllers/User/MemberController.cs
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManagement.Application.DTOs.User;
using GymManagement.Application.DTOs.User.Requests;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/user")]
     [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;

        public MemberController(
            IMemberService memberService,
            IMembershipService membershipService)
        {
            _memberService = memberService;
            _membershipService = membershipService;
        }

        /// <summary>
        /// Helper method to get userId from JWT token
        /// </summary>
        private string GetUserId()
        {
            return User.FindFirst("userId")?.Value 
                   ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng");
        }


        /// <summary>
        /// Get member by ID (public info only)
        /// </summary>
        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberById(string memberId)
        {
            try
            {
                var member = await _memberService.GetMemberByIdAsync(memberId);
                
                // Return only public info
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        id = member.Id,
                        name = member.Name,
                        avatar = member.Avatar
                    }
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
        /// Get current user's profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentProfile()
        
        {
        
            try
            {
                var userId = GetUserId();
                var profile = await _memberService.GetCurrentProfileAsync(userId);
                return Ok(new
                {
                    success = true,
                    data = profile
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
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
        /// Update current user's profile
        /// </summary>
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberUpdateRequest request)
        {
            try
            {
                var userId = GetUserId();
             
                // Validate model state (like Express validator)
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

                var result = await _memberService.UpdateProfileAsync(userId, request);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy thông tin hội viên"
                    });
                }

                // Get updated profile
                var updatedProfile = await _memberService.GetCurrentProfileAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thông tin thành công",
                    data = updatedProfile
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
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
        /// Update profile avatar
        /// </summary>
        [HttpPut("profile/avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] UploadAvatarDto dto)
        {
            try
            {
                if (dto.Avatar == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không tìm thấy file avatar"
                    });
                }

                var userId = GetUserId();
                var newAvatarUrl = await _memberService.UpdateAvatarAsync(userId, dto.Avatar);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật avatar thành công",
                    data = new { avatar = newAvatarUrl }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
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
        /// Update email (requires verification)
        /// </summary>
        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email mới không được để trống"
                    });
                }

                var userId = GetUserId();
                var result = await _memberService.UpdateEmailAsync(userId, request.Email);

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
                    message = "Đã cập nhật email, vui lòng xác thực email mới",
                    data = new { email = request.Email }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
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
        /// Deactivate account (requires password confirmation)
        /// </summary>
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng nhập mật khẩu để xác nhận"
                    });
                }

                var userId = GetUserId();
                var result = await _memberService.DeactivateAccountAsync(userId, request.Password);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
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
        /// get all locations that member could train based on their membership 
        ///</summary>
        [HttpGet("training-locations")]
        public async Task<IActionResult> GetTrainingLocations()
        {
            try
            {
                var userId = GetUserId();
                var locations = await _membershipService.GetMemberTrainingLocationsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = locations.Count,
                    data = locations,
                    message = locations.Count == 0 
                        ? "Bạn chưa đăng ký gói tập nào" 
                        : null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xử lý yêu cầu"
                });
            }
        }

        /// <summary>
        /// get list of member training packages were registered 
        /// get: /api/member/my-package
        /// </summary>
        [HttpGet("my-package")]
        public async Task<IActionResult> GetMyPackages()
        {
            try
            {
                var userId = GetUserId();
                var memberships = await _membershipService.GetMemberMembershipsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = memberships.Count,
                    data = memberships
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xử lý yêu cầu"
                });
            }
        }

        /// <summary>
        /// get list of member active training packages were registered
        /// get: /api/member/my-package-active
        /// </summary>
        /// <returns></returns>
        [HttpGet("my-package-active")]
        public async Task<IActionResult> GetMyActivePackages()
        {
            try
            {
                var userId = GetUserId();
                var memberships = await _membershipService.GetActiveMemberMembershipsAsync(userId);

                return Ok(new
                {
                    success = true,
                    count = memberships.Count,
                    data = memberships
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xử lý yêu cầu"
                });
            }
        }

        /// <summary>
        /// get: /api/member/my-package/detail 
        /// Get detail info of member's packages
        /// </summary>
        [HttpPost("my-package/detail")]
        public async Task<IActionResult> GetMembershipDetail([FromBody] MembershipDetailRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MembershipId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "MembershipId là bắt buộc"
                    });
                }

                var membership = await _membershipService.GetMembershipByIdAsync(request.MembershipId);

                return Ok(new
                {
                    success = true,
                    data = membership
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
        /// patch: /api/member/my-package/pause
        /// Temporary pause a package 
        /// </summary>
        [HttpPatch("my-package/pause")]
        public async Task<IActionResult> PauseMembership([FromBody] PauseMembershipRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MembershipId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "MembershipId là bắt buộc"
                    });
                }

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
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}