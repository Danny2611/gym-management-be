using System.Security.Claims;
using GymManagement.Application.DTOs.Auth;
using GymManagement.Application.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class SocialAuthController : ControllerBase
    {
        private readonly ISocialAuthService _socialAuthService;
        private readonly ILogger<SocialAuthController> _logger;

        public SocialAuthController(
            ISocialAuthService socialAuthService,
            ILogger<SocialAuthController> logger)
        {
            _socialAuthService = socialAuthService;
            _logger = logger;
        }

        // =========================================================
        // GOOGLE
        // =========================================================

        /// <summary>
        /// Bắt đầu đăng nhập Google
        /// </summary>
        /// GET /api/auth/google?callbackUrl=http://localhost:5173/oauth/callback
        [HttpGet("google")]
        public IActionResult GoogleLogin([FromQuery] string callbackUrl)
        {
            // Lưu callbackUrl vào state để lấy lại sau
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(
                    nameof(GoogleResponse),
                    "SocialAuth",
                    new { callbackUrl },
                    protocol: Request.Scheme  // ✅ Thêm này
                ),
                Items =
        {
            { "callbackUrl", callbackUrl }  // ✅ Lưu vào state
        }
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google/response")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string callbackUrl)
        {
            // ✅ Lấy từ properties nếu query param không có
            var result = await HttpContext.AuthenticateAsync("External");

            if (!result.Succeeded || result.Principal == null)
            {
                _logger.LogWarning("Google authentication failed");
                return Redirect($"{callbackUrl}?error=google_login_failed");
            }

            try
            {
                var claims = result.Principal.Claims.ToList();

                // ✅ Log để debug
                _logger.LogInformation("Google claims: {Claims}",
                    string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

                var externalInfo = new ExternalAuthInfo
                {
                    Provider = "Google",
                    ProviderId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        ?? throw new Exception("Missing NameIdentifier"),
                    Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                        ?? throw new Exception("Missing Email"),
                    Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                        ?? "Unknown",
                    Avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value
                };

                var response = await _socialAuthService.HandleExternalLoginAsync(externalInfo);

                // ✅ Sign out external cookie sau khi xong
                await HttpContext.SignOutAsync("External");

                return Redirect(
                    $"{callbackUrl}?accessToken={response.AccessToken}&refreshToken={response.RefreshToken}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google OAuth error");
                return Redirect($"{callbackUrl}?error=internal_error");
            }
        }
        // =========================================================
        // FACEBOOK
        // =========================================================

        /// <summary>
        /// Bắt đầu đăng nhập Facebook
        /// </summary>
        [HttpGet("facebook")]
        public IActionResult FacebookLogin([FromQuery] string callbackUrl)
        {
            var redirectUri = Url.Action(
                nameof(FacebookResponse),
                "SocialAuth",
                new { callbackUrl }
            );

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };

            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Facebook đã login xong → middleware redirect về đây
        /// </summary>
        [HttpGet("facebook/response")]
        public async Task<IActionResult> FacebookResponse([FromQuery] string callbackUrl)
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            if (!result.Succeeded || result.Principal == null)
            {
                return Redirect($"{callbackUrl}?error=facebook_login_failed");
            }

            try
            {
                var claims = result.Principal.Claims;

                var externalInfo = new ExternalAuthInfo
                {
                    Provider = "Facebook",
                    ProviderId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                    Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "",
                    Name = claims.First(c => c.Type == ClaimTypes.Name).Value,
                    Avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value
                };

                var response = await _socialAuthService
                    .HandleExternalLoginAsync(externalInfo);

                return Redirect(
                    $"{callbackUrl}?accessToken={response.AccessToken}&refreshToken={response.RefreshToken}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facebook OAuth error");
                return Redirect($"{callbackUrl}?error=internal_error");
            }
        }
    }
}
