// GymManagement.Application/Services/MemberService.cs
using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Domain.Entities;
using GymManagement.Application.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace GymManagement.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
         private readonly IRoleRepository _roleRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEmailService _emailService;

        public MemberService(
            IRoleRepository roleRepository,
            IMemberRepository memberRepository,
            IFileStorageService fileStorageService,
            IEmailService emailService)
        {
            _memberRepository = memberRepository;
             _roleRepository = roleRepository;
            _fileStorageService = fileStorageService;
            _emailService = emailService;
        }
        

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _memberRepository.GetAllAsync();
        }

        public async Task<Member> GetMemberByIdAsync(string id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                throw new Exception("Member not found");
            }

            return member;
        }

        public async Task<Member> CreateMemberAsync(Member member)
        {
            // Check email duplicate
            var existingMember = await _memberRepository.GetByEmailAsync(member.Email);
            if (existingMember != null)
            {
                throw new Exception("Email already exists");
            }

            member.CreatedAt = DateTime.UtcNow;
            member.UpdatedAt = DateTime.UtcNow;

            return await _memberRepository.CreateAsync(member);
        }

        public async Task<MemberProfileDto> GetCurrentProfileAsync(string userId)
        {
            var member = await _memberRepository.GetByIdAsync(userId);
            if (member == null)
                {
                    throw new Exception("Không tìm thấy thông tin hội viên");
                }
            var role = await _roleRepository.GetByIdAsync(member.Role);
            if (member == null)
            {
                throw new Exception("Không tìm thấy thông tin hội viên");
            }

            return new MemberProfileDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone ?? "",
                Address = member.Address ?? "",
                Avatar = member.Avatar ?? "",
                Gender = member.Gender ?? "",
                Status = member.Status,
                IsVerified = member.IsVerified,
                RoleName = role?.Name?.ToLower() ?? ""
            };
        }

       public async Task<bool> UpdateProfileAsync(string memberId, MemberUpdateRequest request)
            {
                var member = await _memberRepository.GetByIdAsync(memberId);
                if (member == null)
                    return false;

                bool hasChanges = false;

                // Name
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var newName = request.Name.Trim();
                    if (newName != member.Name)
                    {
                        member.Name = newName;
                        hasChanges = true;
                    }
                }

                // Gender
                if (!string.IsNullOrWhiteSpace(request.Gender) &&
                    request.Gender != member.Gender)
                {
                    member.Gender = request.Gender;
                    hasChanges = true;
                }

                // Phone
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    var newPhone = request.Phone.Trim();
                    if (newPhone != member.Phone)
                    {
                        var existingMember = await _memberRepository.GetByPhoneAsync(newPhone);
                        if (existingMember != null && existingMember.Id != memberId)
                        {
                            throw new Exception("Số điện thoại đã được sử dụng bởi tài khoản khác");
                        }

                        member.Phone = newPhone;
                        hasChanges = true;
                    }
                }

                // Date of birth
                if (request.DateOfBirth.HasValue &&
                    request.DateOfBirth != member.DateOfBirth)
                {
                    var minDate = DateTime.UtcNow.AddYears(-10);
                    if (request.DateOfBirth > minDate)
                    {
                        throw new Exception("Hội viên phải ít nhất 10 tuổi");
                    }

                    member.DateOfBirth = request.DateOfBirth;
                    hasChanges = true;
                }

                // Address
                if (!string.IsNullOrWhiteSpace(request.Address))
                {
                    var newAddress = request.Address.Trim();
                    if (newAddress != member.Address)
                    {
                        member.Address = newAddress;
                        hasChanges = true;
                    }
                }

                // Email
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var newEmail = request.Email.Trim().ToLower();
                    if (newEmail != member.Email)
                    {
                        var existingMember = await _memberRepository.GetByEmailAsync(newEmail);
                        if (existingMember != null && existingMember.Id != memberId)
                        {
                            throw new Exception("Email đã được sử dụng bởi tài khoản khác");
                        }

                        member.Email = newEmail;
                        member.IsVerified = false;
                        member.Status = "pending";
                        hasChanges = true;
                    }
                }

                if (!hasChanges)
                    return true;

                member.UpdatedAt = DateTime.UtcNow;
                await _memberRepository.UpdateAsync(member.Id, member);

                return true;
            }


        public async Task<string> UpdateAvatarAsync(string memberId, IFormFile avatarFile)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy thông tin hội viên");
            }

            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(member.Avatar))
            {
                await _fileStorageService.DeleteFileAsync(member.Avatar);
            }

            // Upload new avatar
            var newAvatarPath = await _fileStorageService.UploadAvatarAsync(avatarFile, memberId);

            // Update member's avatar
            member.Avatar = newAvatarPath;
            member.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(member.Id, member);

            return newAvatarPath;
        }

        public async Task<bool> UpdateEmailAsync(string memberId, string newEmail)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
                return false;

            newEmail = newEmail.Trim().ToLower();

            // Check email đã tồn tại
            var exists = await _memberRepository.GetByEmailAsync(newEmail);
            if (exists != null && exists.Id != memberId)
            {
                throw new Exception("Email đã được sử dụng bởi tài khoản khác");
            }

            // Không đổi email → skip
            if (newEmail == member.Email)
                return true;

            // Tạo OTP mới
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            DateTime otpExpires = DateTime.UtcNow.AddMinutes(10);

            // Update member
            member.Email = newEmail;
            member.IsVerified = false;
            member.Status = "pending";
            member.Otp = otp;
            member.OtpExpires = otpExpires;
            member.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(member.Id, member);

            // Gửi email xác thực
            try
            {
                await _emailService.SendChangeEmailOtpAsync(newEmail, otp);
            }
            catch (Exception ex)
            {
                // Log nhưng KHÔNG throw
                Console.WriteLine($"Lỗi gửi email xác thực: {ex.Message}");
            }

            return true;
        }
    

        public async Task<bool> DeactivateAccountAsync(string memberId, string password)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
                return false;

            // Verify password
            if (!member.VerifyPassword(password))
            {
                throw new Exception("Mật khẩu không chính xác");
            }

            member.Status = "inactive";
            member.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(member.Id, member);
            return true;
        }

        public async Task<bool> DeleteMemberAsync(string memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
                return false;

            await _memberRepository.DeleteAsync(memberId);
            return true;
        }
    }
}