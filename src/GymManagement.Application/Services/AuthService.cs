using GymManagement.Application.DTOs.Auth;
using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;
using BCrypt.Net;

namespace GymManagement.Application.Services
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
            var newMember = new Member
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
    }
}