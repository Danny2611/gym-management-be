using GymManagement.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GymManagement.Application.Interfaces.Services.User;
namespace GymManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                return Ok(new
                {
                    success = true,
                    data = response,
                    message = "Đăng nhập thành công"
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
        /// Đăng ký
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);

                return Ok(new
                {
                    success = true,
                    data = response,
                    message = response.Message
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
        /// Xác thực OTP
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            try
            {
                var response = await _authService.VerifyOTPAsync(request);

                return Ok(new
                {
                    success = true,
                    data = response,
                    message = "Xác thực tài khoản thành công"
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

        [HttpPost("verify-otp-forgot-password")]
        public async Task<IActionResult> VerifyOTPForgotPassword([FromBody] VerifyOTPRequest request)
        {
            try
            {
                await _authService.VerifyOTPForgotPasswordAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Xác thực OTP thành công"
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
        /// Gửi lại OTP
        /// </summary>
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOTPRequest request)
        {
            try
            {
                await _authService.ResendOTPAsync(request.Email);

                return Ok(new
                {
                    success = true,
                    message = "Đã gửi lại mã OTP. Vui lòng kiểm tra email."
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
        /// Quên mật khẩu - Gửi OTP
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request.Email);

                return Ok(new
                {
                    success = true,
                    message = "Đã gửi mã OTP đến email của bạn. Vui lòng kiểm tra email."
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
        /// Reset mật khẩu với OTP
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công"
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
        /// Đổi mật khẩu (yêu cầu đăng nhập)
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập"
                    });
                }

                await _authService.ChangePasswordAsync(userId, request);

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công"
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
        /// Validate mật khẩu hiện tại (yêu cầu đăng nhập)
        /// </summary>
        [Authorize]
        [HttpPost("validate-current-password")]
        public async Task<IActionResult> ValidateCurrentPassword([FromBody] ValidatePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập"
                    });
                }

                var isValid = await _authService.ValidateCurrentPasswordAsync(userId, request.CurrentPassword);

                return Ok(new
                {
                    success = true,
                    data = new { isValid },
                    message = isValid ? "Mật khẩu đúng" : "Mật khẩu không đúng"
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