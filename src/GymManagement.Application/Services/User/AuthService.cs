using GymManagement.Application.DTOs.Auth;
using BCrypt.Net;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{
    public class AuthService : IAuthService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;

        public AuthService(
            IMemberRepository memberRepository,
            IRoleRepository roleRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService)
        {
            _memberRepository = memberRepository;
            _roleRepository = roleRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // 1. Tìm người dùng
            var user = await _memberRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }

            // 2. Kiểm tra mật khẩu
            bool isMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isMatch)
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }

            // 3. Kiểm tra xác thực
            if (!user.IsVerified)
            {
                throw new Exception("Tài khoản chưa được xác thực. Vui lòng kiểm tra email");
            }

            // 4. Kiểm tra trạng thái
            if (user.Status != "active")
            {
                throw new Exception("Tài khoản đã bị khóa");
            }

            // 5. Lấy thông tin vai trò
            var role = await _roleRepository.GetByIdAsync(user.Role);
            var roleName = role?.Name ?? "Member";

            // 6. Tạo token
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, roleName);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, roleName);

            // 7. Return response
            return new LoginResponse
            {
                User = new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    RoleName = roleName,
                    Avatar = user.Avatar
                },
                Tokens = new TokenInfo
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Check email đã tồn tại chưa
            var existingMember = await _memberRepository.GetByEmailAsync(request.Email);

            if (existingMember != null)
            {
                throw new Exception("Email đã được sử dụng");
            }

            // 2. Lấy role mặc định "Member"
            var memberRole = await _roleRepository.GetByNameAsync("Member");
            if (memberRole == null)
            {
                throw new Exception("Role Member không tồn tại trong hệ thống");
            }

            // 3. Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Tạo OTP (6 số)
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            DateTime otpExpires = DateTime.UtcNow.AddMinutes(10); // OTP hết hạn sau 10 phút

            // 5. Tạo member mới
            var newMember = new Domain.Entities.Member
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Phone = request.Phone,
                Role = memberRole.Id,
                Status = "pending", // Chờ xác thực
                IsVerified = false,
                Otp = otp,
                OtpExpires = otpExpires,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdMember = await _memberRepository.CreateAsync(newMember);

            // 6. Gửi email OTP
            try
            {
                await _emailService.SendOTPEmailAsync(request.Email, otp);
            }
            catch (Exception ex)
            {
                // Log error nhưng không throw để user vẫn đăng ký được
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
            }

            // 7. Return response
            return new RegisterResponse
            {
                UserId = createdMember.Id,
                Email = createdMember.Email,
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản."
            };
        }

        public async Task<LoginResponse> VerifyOTPAsync(VerifyOTPRequest request)
        {
            // 1. Tìm user theo email
            var user = await _memberRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại");
            }

            // 2. Kiểm tra user đã verified chưa
            if (user.IsVerified)
            {
                throw new Exception("Tài khoản đã được xác thực");
            }

            // 3. Kiểm tra OTP
            if (user.Otp != request.Otp)
            {
                throw new Exception("Mã OTP không đúng");
            }

            // 4. Kiểm tra OTP hết hạn chưa
            if (user.OtpExpires < DateTime.UtcNow)
            {
                throw new Exception("Mã OTP đã hết hạn");
            }

            // 5. Update user: verified = true, status = active, xóa OTP
            user.IsVerified = true;
            user.Status = "active";
            user.Otp = null;
            user.OtpExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

            // 6. Gửi email chào mừng
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email chào mừng: {ex.Message}");
            }

            // 7. Lấy thông tin vai trò
            var role = await _roleRepository.GetByIdAsync(user.Role);
            var roleName = role?.Name ?? "Member";

            // 8. Tạo token tự động đăng nhập
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, roleName);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, roleName);

            // 9. Return response
            return new LoginResponse
            {
                User = new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    RoleName = roleName,
                    Avatar = user.Avatar
                },
                Tokens = new TokenInfo
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public async Task ResendOTPAsync(string email)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại");
            }

            // 2. Kiểm tra đã verified chưa
            if (user.IsVerified)
            {
                throw new Exception("Tài khoản đã được xác thực");
            }

            // 3. Tạo OTP mới
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            DateTime otpExpires = DateTime.UtcNow.AddMinutes(10);

            // 4. Update user
            user.Otp = otp;
            user.OtpExpires = otpExpires;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

            // 5. Gửi email OTP
            await _emailService.SendOTPEmailAsync(email, otp);
        }




        /// <summary>
        /// Quên mật khẩu - Gửi OTP qua email
        /// </summary>
        public async Task ForgotPasswordAsync(string email)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại trong hệ thống");
            }

            // 2. Kiểm tra user đã verified chưa
            if (!user.IsVerified)
            {
                throw new Exception("Tài khoản chưa được xác thực. Vui lòng xác thực tài khoản trước.");
            }

            // 3. Kiểm tra trạng thái
            if (user.Status != "active")
            {
                throw new Exception("Tài khoản đã bị khóa");
            }

            // 4. Tạo OTP
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            DateTime otpExpires = DateTime.UtcNow.AddMinutes(10);

            // 5. Update user
            user.Otp = otp;
            user.OtpExpires = otpExpires;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

            // 6. Gửi email OTP
            await _emailService.SendResetPasswordOtpAsync(email, otp);
        }


        /// <summary>
        /// Reset mật khẩu với OTP
        /// </summary>
        public async Task VerifyOTPForgotPasswordAsync(VerifyOTPRequest request)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại");
            }

            // 2. Kiểm tra OTP
            if (user.Otp != request.Otp)
            {
                throw new Exception("Mã OTP không đúng");
            }

            // 3. Kiểm tra OTP hết hạn
            if (user.OtpExpires < DateTime.UtcNow)
            {
                throw new Exception("Mã OTP đã hết hạn");
            }
            user.Otp = null;
            user.OtpExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

        }


        /// <summary>
        /// Reset mật khẩu với OTP
        /// </summary>
        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại");
            }
            // 4. Hash password mới
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 5. Update user
            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

            // 6. Gửi email thông báo
            await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.Name);
        }

        /// <summary>
        /// Đổi mật khẩu (cho user đã đăng nhập)
        /// </summary>
        public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            // 2. Verify mật khẩu hiện tại
            bool isMatch = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);

            if (!isMatch)
            {
                throw new Exception("Mật khẩu hiện tại không đúng");
            }

            // 3. Kiểm tra mật khẩu mới không trùng mật khẩu cũ
            if (request.CurrentPassword == request.NewPassword)
            {
                throw new Exception("Mật khẩu mới không được trùng với mật khẩu hiện tại");
            }

            // 4. Hash password mới
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 5. Update user
            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(user.Id, user);

            // 6. Gửi email thông báo
            await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.Name);
        }

        /// <summary>
        /// Validate mật khẩu hiện tại
        /// </summary>
        public async Task<bool> ValidateCurrentPasswordAsync(string userId, string currentPassword)
        {
            // 1. Tìm user
            var user = await _memberRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            // 2. Verify password
            return BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
        }

        /// <summary>
        /// Google Callback - Xử lý đăng nhập Google
        /// </summary>
        public async Task<GoogleCallbackResponse> GoogleCallbackAsync(string googleUserId, string email, string name, string? avatar)
        {
            // 1. Tìm user theo email
            var user = await _memberRepository.GetByEmailAsync(email);

            // 2. Nếu chưa có user, tạo mới
            if (user == null)
            {
                // Lấy role mặc định
                var memberRole = await _roleRepository.GetByNameAsync("Member");
                if (memberRole == null)
                {
                    throw new Exception("Role Member không tồn tại trong hệ thống");
                }

                // Tạo user mới từ Google
                user = new Member
                {
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password
                    Avatar = avatar,
                    Role = memberRole.Id,
                    Status = "active",
                    IsVerified = true, // Google account đã verified
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                user = await _memberRepository.CreateAsync(user);

                // Gửi email chào mừng
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi email chào mừng: {ex.Message}");
                }
            }
            else
            {
                // 3. Nếu đã có user, kiểm tra trạng thái
                if (user.Status != "active")
                {
                    throw new Exception("Tài khoản đã bị khóa");
                }

                // Update avatar nếu có
                if (!string.IsNullOrEmpty(avatar) && user.Avatar != avatar)
                {
                    user.Avatar = avatar;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _memberRepository.UpdateAsync(user.Id, user);
                }
            }

            // 4. Lấy thông tin vai trò
            var role = await _roleRepository.GetByIdAsync(user.Role);
            var roleName = role?.Name ?? "Member";

            // 5. Tạo tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, roleName);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, roleName);

            // 6. Return response
            return new GoogleCallbackResponse
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