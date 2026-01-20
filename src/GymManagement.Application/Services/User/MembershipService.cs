
using GymManagement.Application.DTOs.User;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.User;



namespace GymManagement.Application.Services.User
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageDetailRepository _packageDetailRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPaymentRepository _paymentRepository;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IPackageRepository packageRepository,
            IPackageDetailRepository packageDetailRepository,
            IMemberRepository memberRepository,
            IPaymentRepository paymentRepository)
        {
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
            _packageDetailRepository = packageDetailRepository;
            _memberRepository = memberRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<List<MembershipResponse>> GetMembershipsAsync(string memberId)
        {
            var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
            var result = new List<MembershipResponse>();

            foreach (var membership in memberships.OrderByDescending(m => m.CreatedAt))
            {
                var package = await _packageRepository.GetByIdAsync(membership.PackageId);
                var payment = await _paymentRepository.GetByIdAsync(membership.PaymentId);
                var packageDetails = package != null
                    ? await _packageDetailRepository.GetByPackageIdAsync(package.Id)
                    : null;

                var membershipResponse = new MembershipResponse
                {
                    Id = membership.Id,
                    Member_id = membership.MemberId,
                    Start_date = membership.StartDate,
                    End_date = membership.EndDate,
                    AutoRenew = membership.AutoRenew,
                    Status = membership.Status,
                    Available_sessions = membership.AvailableSessions,
                    Used_sessions = membership.UsedSessions,
                    Last_sessions_reset = membership.LastSessionsReset,
                    Created_at = membership.CreatedAt,
                    Updated_at = membership.UpdatedAt
                };

                // Map Package Info
                if (package != null)
                {
                    membershipResponse.Package_id = new PackageInfoDto
                    {
                        Id = package.Id,
                        Name = package.Name,
                        Price = package.Price,
                        Duration = package.Duration,
                        Max_members = package.MaxMembers,
                        Description = package.Description,
                        Benefits = package.Benefits,
                        Status = package.Status,
                        Category = package.Category,
                        Popular = package.Popular,
                        Training_sessions = package.TrainingSessions,
                        Session_duration = package.SessionDuration
                    };

                    // Map Package Details nếu có
                    if (packageDetails != null)
                    {
                        membershipResponse.Package_id.Details = new PackageDetailsDto
                        {
                            Id = packageDetails.Id,
                            Package_id = packageDetails.PackageId,
                            Schedule = packageDetails.Schedule,
                            Training_areas = packageDetails.TrainingAreas,
                            Additional_services = packageDetails.AdditionalServices,
                            Status = packageDetails.Status
                        };
                    }
                }

                // Map Payment Info
                if (payment != null)
                {
                    membershipResponse.Payment_id = new PaymentInfoDto
                    {
                        Id = payment.Id,
                        PaymentMethod = payment.PaymentMethod,
                        Amount = payment.Amount,
                        Payment_date = payment.CreatedAt,
                        Status = payment.Status
                    };

                    // Extract PaymentInfo từ BsonDocument
                    if (payment.PaymentInfo != null)
                    {
                        membershipResponse.Payment_id.PaymentInfo = new PaymentInfoDetailsDto
                        {
                            ResponseTime = payment.PaymentInfo.Contains("responseTime")
        ? payment.PaymentInfo["responseTime"].ToString()
        : string.Empty,

                            Message = payment.PaymentInfo.Contains("message")
        ? payment.PaymentInfo["message"].ToString()
        : string.Empty
                        };

                    }
                }

                // Calculate remaining days and percent
                CalculateMembershipRemaining(membershipResponse);

                result.Add(membershipResponse);
            }

            return result;
        }


        /// <summary>
        /// Lấy chi tiết membership theo ID
        /// </summary>
        public async Task<MembershipResponse> GetMembershipByIdAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            var package = await _packageRepository.GetByIdAsync(membership.PackageId);
            var payment = await _paymentRepository.GetByIdAsync(membership.PaymentId);
            var packageDetails = package != null
                ? await _packageDetailRepository.GetByPackageIdAsync(package.Id)
                : null;

            var membershipResponse = new MembershipResponse
            {
                Id = membership.Id,
                Member_id = membership.MemberId,
                Start_date = membership.StartDate,
                End_date = membership.EndDate,
                AutoRenew = membership.AutoRenew,
                Status = membership.Status,
                Available_sessions = membership.AvailableSessions,
                Used_sessions = membership.UsedSessions,
                Last_sessions_reset = membership.LastSessionsReset,
                Created_at = membership.CreatedAt,
                Updated_at = membership.UpdatedAt
            };

            // Map Package Info
            if (package != null)
            {
                membershipResponse.Package_id = new PackageInfoDto
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    Max_members = package.MaxMembers,
                    Description = package.Description,
                    Benefits = package.Benefits,
                    Status = package.Status,
                    Category = package.Category,
                    Popular = package.Popular,
                    Training_sessions = package.TrainingSessions,
                    Session_duration = package.SessionDuration
                };

                // Map Package Details nếu có
                if (packageDetails != null)
                {
                    membershipResponse.Package_id.Details = new PackageDetailsDto
                    {
                        Id = packageDetails.Id,
                        Package_id = packageDetails.PackageId,
                        Schedule = packageDetails.Schedule,
                        Training_areas = packageDetails.TrainingAreas,
                        Additional_services = packageDetails.AdditionalServices,
                        Status = packageDetails.Status
                    };
                }
            }

            // Map Payment Info
            if (payment != null)
            {
                membershipResponse.Payment_id = new PaymentInfoDto
                {
                    Id = payment.Id,
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Payment_date = payment.CreatedAt,
                    Status = payment.Status
                };

                // Extract PaymentInfo từ BsonDocument
                if (payment.PaymentInfo != null)
                {
                    membershipResponse.Payment_id.PaymentInfo = new PaymentInfoDetailsDto
                    {
                        ResponseTime = payment.PaymentInfo.Contains("responseTime")
         ? payment.PaymentInfo["responseTime"].ToString()
         : string.Empty,

                        Message = payment.PaymentInfo.Contains("message")
         ? payment.PaymentInfo["message"].ToString()
         : string.Empty
                    };

                }
            }

            // Calculate remaining days and percent
            CalculateMembershipRemaining(membershipResponse);

            return membershipResponse;
        }

        private void CalculateMembershipRemaining(MembershipResponse membership)
        {
            if (membership.Start_date.HasValue && membership.End_date.HasValue)
            {
                var now = DateTime.UtcNow;
                var totalDays = (membership.End_date.Value - membership.Start_date.Value).TotalDays;
                var remainingDays = (membership.End_date.Value - now).TotalDays;

                membership.RemainingDays = remainingDays > 0 ? (int)Math.Ceiling(remainingDays) : 0;

                if (totalDays > 0)
                {
                    var usedDays = (now - membership.Start_date.Value).TotalDays;
                    membership.RemainingPercent = Math.Max(0, Math.Min(100, ((totalDays - usedDays) / totalDays) * 100));
                }
                else
                {
                    membership.RemainingPercent = 0;
                }
            }
            else
            {
                membership.RemainingDays = 0;
                membership.RemainingPercent = 0;
            }
        }




        /// <summary>
        /// Đăng ký gói tập - Kiểm tra trước khi thanh toán
        /// </summary>
        public async Task<RegisterPackageResponse> RegisterPackageAsync(string memberId, RegisterPackageRequest request)
        {
            // 1. Kiểm tra gói tập tồn tại
            var packageInfo = await _packageRepository.GetByIdAsync(request.PackageId);
            if (packageInfo == null)
            {
                throw new Exception("Không tìm thấy gói tập");
            }

            // 2. Kiểm tra trạng thái gói tập
            if (packageInfo.Status != "active")
            {
                throw new Exception("Gói tập này hiện không khả dụng");
            }

            // 3. Kiểm tra đã đăng ký chưa
            var existingMembership = await _membershipRepository.GetByMemberAndPackageAsync(
                memberId,
                request.PackageId,
                "active"
            );

            if (existingMembership != null)
            {
                throw new Exception("Bạn đã đăng ký gói tập này rồi. Vui lòng chọn gói tập khác!!");
            }

            // 4. Trả về thông tin gói tập để tiến hành thanh toán
            return new RegisterPackageResponse
            {
                PackageId = packageInfo.Id,
                PackageName = packageInfo.Name,
                Price = packageInfo.Price,
                Duration = packageInfo.Duration,
                Category = packageInfo.Category
            };
        }

        /// <summary>
        /// Lấy danh sách địa điểm tập luyện của member
        /// </summary>
        public async Task<List<string>> GetMemberTrainingLocationsAsync(string memberId)
        {
            // 1. Lấy memberships đang active
            var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
            var activeMemberships = memberships.Where(m => m.Status == "active").ToList();

            if (!activeMemberships.Any())
            {
                return new List<string>();
            }

            // 2. Lấy packageIds
            var packageIds = activeMemberships.Select(m => m.PackageId).ToList();

            // 3. Lấy PackageDetails
            var packageDetails = await _packageDetailRepository.GetByPackageIdsAsync(packageIds);

            // 4. Lấy unique training locations
            var locations = packageDetails
                .SelectMany(pd => pd.TrainingAreas)
                .Distinct()
                .ToList();

            return locations;
        }

        /// <summary>
        /// Lấy tất cả memberships của member
        /// </summary>

        /// <summary>
        /// Lấy memberships đang active
        /// </summary>
        public async Task<List<MembershipResponse>> GetActiveMembershipsAsync(string memberId)
        {
            var allMemberships = await GetMembershipsAsync(memberId);
            return allMemberships.Where(m => m.Status == "active").ToList();
        }



        /// <summary>
        /// Tạm dừng membership
        /// </summary>
        public async Task<MembershipResponse> PauseMembershipAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            membership.Status = "paused";
            membership.StartDate = null;
            await _membershipRepository.UpdateAsync(membership.Id, membership);

            return await GetMembershipByIdAsync(membershipId);
        }

        /// <summary>
        /// Kích hoạt lại membership
        /// </summary>
        public async Task<MembershipResponse> ResumeMembershipAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            if (membership.Status != "paused")
            {
                throw new Exception("Gói tập này không ở trạng thái tạm dừng");
            }

            membership.Status = "active";
            membership.StartDate = DateTime.UtcNow;
            await _membershipRepository.UpdateAsync(membership.Id, membership);

            return await GetMembershipByIdAsync(membershipId);
        }

        /// <summary>
        /// Lấy thông tin chi tiết membership (giống getMembershipDetails của Express)
        /// </summary>
        public async Task<MembershipDetailsResponse> GetMembershipDetailsAsync(string memberId)
        {
            // 1. Lấy thông tin member
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new Exception("Không tìm thấy hội viên");
            }

            // 2. Lấy memberships (active hoặc expired)
            var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
            var validMemberships = memberships
                .Where(m => m.Status == "active" || m.Status == "expired")
                .ToList();

            // 3. Nếu không có membership nào
            if (!validMemberships.Any())
            {
                return new MembershipDetailsResponse
                {
                    MembershipId = "null",
                    MemberName = member.Name,
                    MemberAvatar = member.Avatar ?? "/placeholder-avatar.jpg",
                    PackageId = "",
                    PackageName = "Chưa đăng ký",
                    PackageCategory = "basic",
                    Status = "null",
                    DaysRemaining = 0,
                    SessionsRemaining = 0,
                    TotalSessions = 0
                };
            }

            // 4. Tìm membership active trước, không có thì lấy expired
            var selectedMembership = validMemberships.FirstOrDefault(m => m.Status == "active")
                                   ?? validMemberships.FirstOrDefault(m => m.Status == "expired")
                                   ?? validMemberships.First();

            // 5. Tìm membership có giá cao nhất
            var packages = new List<Domain.Entities.Package>();
            foreach (var m in validMemberships)
            {
                var pkg = await _packageRepository.GetByIdAsync(m.PackageId);
                if (pkg != null) packages.Add(pkg);
            }

            var highestPricePackage = packages.OrderByDescending(p => p.Price).FirstOrDefault();
            var highestPriceMembership = validMemberships.FirstOrDefault(m =>
                m.PackageId == highestPricePackage?.Id) ?? selectedMembership;

            // 6. Tính số ngày còn lại
            int daysRemaining = 0;
            if (highestPriceMembership.EndDate.HasValue)
            {
                var diff = (highestPriceMembership.EndDate.Value - DateTime.UtcNow).TotalDays;
                daysRemaining = Math.Max(0, (int)Math.Ceiling(diff));
            }

            // 7. Tính số buổi tập còn lại
            var sessionsRemaining = highestPriceMembership.AvailableSessions - highestPriceMembership.UsedSessions;

            // 8. Lấy thông tin package
            var packageInfo = await _packageRepository.GetByIdAsync(selectedMembership.PackageId);

            return new MembershipDetailsResponse
            {
                MembershipId = selectedMembership.Id,
                MemberName = member.Name,
                MemberAvatar = member.Avatar ?? "/placeholder-avatar.jpg",
                PackageId = packageInfo?.Id ?? "",
                PackageName = packageInfo?.Name ?? "Không có thông tin",
                PackageCategory = packageInfo?.Category ?? "basic",
                Status = selectedMembership.Status,
                DaysRemaining = daysRemaining,
                SessionsRemaining = sessionsRemaining,
                TotalSessions = highestPriceMembership.AvailableSessions
            };
        }

        /// <summary>
        /// Tự động cập nhật memberships hết hạn
        /// </summary>
        public async Task UpdateExpiredMembershipsAsync()
        {
            try
            {
                var allMemberships = await _membershipRepository.GetByMemberIdAsync("");
                var today = DateTime.UtcNow;
                int updatedCount = 0;

                foreach (var membership in allMemberships)
                {
                    if (membership.Status == "active" &&
                        membership.EndDate.HasValue &&
                        membership.EndDate.Value < today)
                    {
                        membership.Status = "expired";
                        await _membershipRepository.UpdateAsync(membership.Id, membership);
                        updatedCount++;
                    }
                }

                Console.WriteLine($"Đã cập nhật {updatedCount} membership hết hạn.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật membership hết hạn: {ex.Message}");
            }
        }
    }
}