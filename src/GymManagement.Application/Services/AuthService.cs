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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IMemberRepository memberRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _memberRepository = memberRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // 1. Tìm user theo email
            var member = await _memberRepository.GetByEmailAsync(request.Email);
            
            if (member == null)
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }

            // 2. Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, member.Password);
            
            if (!isPasswordValid)
            {
                throw new Exception("Email hoặc mật khẩu không đúng");
            }

            // 3. Check status
            if (member.Status != "active")
            {
                throw new Exception("Tài khoản đã bị khóa hoặc chưa được kích hoạt");
            }

            // 4. Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(member);
            var expiresAt = _jwtTokenGenerator.GetTokenExpiry();

            // 5. Return response
            return new LoginResponse
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Check email đã tồn tại chưa
            var existingMember = await _memberRepository.GetByEmailAsync(request.Email);
            
            if (existingMember != null)
            {
                throw new Exception("Email đã được sử dụng");
            }

            // 2. Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo member mới
            var newMember = new Member
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Phone = request.Phone,
                Gender = request.Gender,
                Status = "active",
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdMember = await _memberRepository.CreateAsync(newMember);

            // 4. Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(createdMember);
            var expiresAt = _jwtTokenGenerator.GetTokenExpiry();

            // 5. Return response
            return new LoginResponse
            {
                Id = createdMember.Id,
                Name = createdMember.Name,
                Email = createdMember.Email,
                Role = createdMember.Role,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}