
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings.Admin;

namespace GymManagement.Application.Services.Admin
{
    public class AdminMembershipService : IAdminMembershipService
    {
        private readonly IAdminMembershipRepository _membershipRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPackageRepository _packageRepository;

        public AdminMembershipService(
            IAdminMembershipRepository membershipRepository,
            IMemberRepository memberRepository,
            IPackageRepository packageRepository)
        {
            _membershipRepository = membershipRepository;
            _memberRepository = memberRepository;
            _packageRepository = packageRepository;
        }

        public async Task<MembershipListResponseDto> GetAllMembershipsAsync(
            MembershipQueryOptions options)
        {
            var (memberships, totalCount) = await _membershipRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            // Populate members and packages
            var membershipDtos = new List<MembershipResponseDto>();

            foreach (var membership in memberships)
            {
                var member = await _memberRepository.GetByIdAsync(membership.MemberId);
                var package = await _packageRepository.GetByIdAsync(membership.PackageId);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(options.Search))
                {
                    var searchLower = options.Search.ToLower();
                    var memberName = member?.Name?.ToLower() ?? "";
                    var memberEmail = member?.Email?.ToLower() ?? "";
                    var packageName = package?.Name?.ToLower() ?? "";

                    if (!memberName.Contains(searchLower) &&
                        !memberEmail.Contains(searchLower) &&
                        !packageName.Contains(searchLower))
                    {
                        continue;
                    }
                }

                if (member != null && package != null)
                {
                    membershipDtos.Add(membership.ToDto(member, package));
                }
            }

            return new MembershipListResponseDto
            {
                Memberships = membershipDtos,
                TotalMemberships = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<MembershipResponseDto> GetMembershipByIdAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);

            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đăng ký thành viên");
            }

            var member = await _memberRepository.GetByIdAsync(membership.MemberId);
            var package = await _packageRepository.GetByIdAsync(membership.PackageId);

            if (member == null || package == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin member hoặc package");
            }

            return membership.ToDto(member, package);
        }

        public async Task<bool> DeleteMembershipAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);

            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy id của membership");
            }

            // Kiểm tra điều kiện: status === 'pending' và created_at > 1 tuần trước
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var isOlderThanAWeek = membership.CreatedAt < oneWeekAgo;

            if (membership.Status != "pending" || !isOlderThanAWeek)
            {
                throw new InvalidOperationException("Chỉ được xóa các đăng ký đang chờ và đã tạo hơn 1 tuần");
            }

            return await _membershipRepository.DeleteAsync(membershipId);
        }

        public async Task<MembershipStatsDto> GetMembershipStatsAsync()
        {
            return await _membershipRepository.GetStatsAsync();
        }
    }
}