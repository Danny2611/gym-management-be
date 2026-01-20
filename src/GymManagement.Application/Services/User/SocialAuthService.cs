using GymManagement.Application.DTOs.Auth;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{

    public class SocialAuthService : ISocialAuthService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public SocialAuthService(
            IMemberRepository memberRepository,
            IRoleRepository roleRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _memberRepository = memberRepository;
            _roleRepository = roleRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<SocialAuthCallbackResponse> HandleExternalLoginAsync(ExternalAuthInfo externalInfo)
        {
            // 1. Tìm hoặc tạo user
            var user = await _memberRepository.GetByEmailAsync(externalInfo.Email);

            if (user == null)
            {
                // Tạo user mới từ thông tin OAuth
                var defaultRole = await _roleRepository.GetByNameAsync("Member");

                if (defaultRole == null)
                {
                    throw new Exception("Không tìm thấy vai trò mặc định");
                }

                user = new Member
                {
                    Name = externalInfo.Name,
                    Email = externalInfo.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword($"{externalInfo.Provider.ToLower()}-auth-{Guid.NewGuid()}"), // Random password
                    Role = defaultRole.Id,
                    Avatar = externalInfo.Avatar,
                    IsVerified = true, // OAuth đã verify email
                    Status = "active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _memberRepository.CreateAsync(user);
            }
            else
            {
                // User đã tồn tại - cập nhật avatar nếu chưa có
                if (string.IsNullOrEmpty(user.Avatar) && !string.IsNullOrEmpty(externalInfo.Avatar))
                {
                    user.Avatar = externalInfo.Avatar;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _memberRepository.UpdateAsync(user.Id, user);
                }
            }

            // 2. Kiểm tra trạng thái tài khoản
            if (user.Status != "active")
            {
                throw new Exception("Tài khoản đã bị khóa");
            }

            // 3. Lấy thông tin vai trò
            var role = await _roleRepository.GetByIdAsync(user.Role);
            var roleName = role?.Name ?? "Member";

            // 4. Tạo tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, roleName);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, roleName);

            // 5. Return response
            return new SocialAuthCallbackResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserData = new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    RoleName = roleName,
                    Avatar = user.Avatar
                }
            };
        }
    }
}