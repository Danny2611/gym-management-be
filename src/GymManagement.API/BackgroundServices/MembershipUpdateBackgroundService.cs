// using GymManagement.Application.DTOs.Membership;
// using GymManagement.Application.Interfaces.Repositories.User;
// namespace GymManagement.Application.Interfaces.Repositories.User;
// using MongoDB.Driver;

// namespace GymManagement.Application.Services.User
// {
//     public class MembershipService : IMembershipService
//     {
//         private readonly IMembershipRepository _membershipRepository;
//         private readonly IPackageRepository _packageRepository;
//         private readonly IPackageDetailRepository _packageDetailRepository;
//         private readonly IMemberRepository _memberRepository;
//         private readonly IPaymentRepository _paymentRepository;

//         public MembershipService(
//             IMembershipRepository membershipRepository,
//             IPackageRepository packageRepository,
//             IPackageDetailRepository packageDetailRepository,
//             IMemberRepository memberRepository,
//             IPaymentRepository paymentRepository)
//         {
//             _membershipRepository = membershipRepository;
//             _packageRepository = packageRepository;
//             _packageDetailRepository = packageDetailRepository;
//             _memberRepository = memberRepository;
//             _paymentRepository = paymentRepository;
//         }

//         /// <summary>
//         /// Đăng ký gói tập - Kiểm tra trước khi thanh toán
//         /// </summary>
//         public async Task<RegisterPackageResponse> RegisterPackageAsync(string memberId, RegisterPackageRequest request)
//         {
//             // 1. Kiểm tra gói tập tồn tại
//             var packageInfo = await _packageRepository.GetByIdAsync(request.PackageId);
//             if (packageInfo == null)
//             {
//                 throw new Exception("Không tìm thấy gói tập");
//             }

//             // 2. Kiểm tra trạng thái gói tập
//             if (packageInfo.Status != "active")
//             {
//                 throw new Exception("Gói tập này hiện không khả dụng");
//             }

//             // 3. Kiểm tra đã đăng ký chưa
//             var existingMembership = await _membershipRepository.GetByMemberAndPackageAsync(
//                 memberId,
//                 request.PackageId,
//                 "active"
//             );

//             if (existingMembership != null)
//             {
//                 throw new Exception("Bạn đã đăng ký gói tập này rồi. Vui lòng chọn gói tập khác!!");
//             }

//             // 4. Trả về thông tin gói tập để tiến hành thanh toán
//             return new RegisterPackageResponse
//             {
//                 PackageId = packageInfo.Id,
//                 PackageName = packageInfo.Name,
//                 Price = packageInfo.Price,
//                 Duration = packageInfo.Duration,
//                 Category = packageInfo.Category
//             };
//         }

//         /// <summary>
//         /// Lấy danh sách địa điểm tập luyện của member
//         /// </summary>
//         public async Task<List<string>> GetMemberTrainingLocationsAsync(string memberId)
//         {
//             // 1. Lấy memberships đang active
//             var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
//             var activeMemberships = memberships.Where(m => m.Status == "active").ToList();

//             if (!activeMemberships.Any())
//             {
//                 return new List<string>();
//             }

//             // 2. Lấy packageIds
//             var packageIds = activeMemberships.Select(m => m.PackageId).ToList();

//             // 3. Lấy PackageDetails
//             var packageDetails = await _packageDetailRepository.GetByPackageIdsAsync(packageIds);

//             // 4. Lấy unique training locations
//             var locations = packageDetails
//                 .SelectMany(pd => pd.TrainingAreas)
//                 .Distinct()
//                 .ToList();

//             return locations;
//         }

//         /// <summary>
//         /// Lấy tất cả memberships của member
//         /// </summary>
//         public async Task<List<MembershipResponse>> GetMembershipsAsync(string memberId)
//         {
//             var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);

//             var result = new List<MembershipResponse>();

//             foreach (var membership in memberships.OrderByDescending(m => m.CreatedAt))
//             {
//                 var package = await _packageRepository.GetByIdAsync(membership.PackageId);
//                 var payment = await _paymentRepository.GetByIdAsync(membership.PaymentId);

//                 result.Add(new MembershipResponse
//                 {
//                     Id = membership.Id,
//                     MemberId = membership.MemberId,
//                     Package = package != null ? new PackageInfoDto
//                     {
//                         Id = package.Id,
//                         Name = package.Name,
//                         Price = package.Price,
//                         Duration = package.Duration,
//                         Category = package.Category
//                     } : null,
//                     Payment = payment != null ? new PaymentInfoDto
//                     {
//                         Id = payment.Id,
//                         Amount = payment.Amount,
//                         Status = payment.Status,
//                         PaymentMethod = payment.PaymentMethod
//                     } : null,
//                     StartDate = membership.StartDate,
//                     EndDate = membership.EndDate,
//                     Status = membership.Status,
//                     AvailableSessions = membership.AvailableSessions,
//                     UsedSessions = membership.UsedSessions,
//                     CreatedAt = membership.CreatedAt
//                 });
//             }

//             return result;
//         }

//         /// <summary>
//         /// Lấy memberships đang active
//         /// </summary>
//         public async Task<List<MembershipDto>> GetActiveMembershipsAsync(string memberId)
//         {
//             var allMemberships = await GetMembershipsAsync(memberId);
//             return allMemberships.Where(m => m.Status == "active").ToList();
//         }

//         /// <summary>
//         /// Lấy chi tiết membership theo ID
//         /// </summary>
//         public async Task<MembershipDto> GetMembershipByIdAsync(string membershipId)
//         {
//             var membership = await _membershipRepository.GetByIdAsync(membershipId);
//             if (membership == null)
//             {
//                 throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
//             }

//             var package = await _packageRepository.GetByIdAsync(membership.PackageId);
//             var payment = await _paymentRepository.GetByIdAsync(membership.PaymentId);

//             return new MembershipDto
//             {
//                 Id = membership.Id,
//                 MemberId = membership.MemberId,
//                 Package = package != null ? new PackageInfoDto
//                 {
//                     Id = package.Id,
//                     Name = package.Name,
//                     Price = package.Price,
//                     Duration = package.Duration,
//                     Category = package.Category
//                 } : null,
//                 Payment = payment != null ? new PaymentInfoDto
//                 {
//                     Id = payment.Id,
//                     Amount = payment.Amount,
//                     Status = payment.Status,
//                     PaymentMethod = payment.PaymentMethod
//                 } : null,
//                 StartDate = membership.StartDate,
//                 EndDate = membership.EndDate,
//                 Status = membership.Status,
//                 AvailableSessions = membership.AvailableSessions,
//                 UsedSessions = membership.UsedSessions,
//                 CreatedAt = membership.CreatedAt
//             };
//         }

//         /// <summary>
//         /// Tạm dừng membership
//         /// </summary>
//         public async Task<MembershipDto> PauseMembershipAsync(string membershipId)
//         {
//             var membership = await _membershipRepository.GetByIdAsync(membershipId);
//             if (membership == null)
//             {
//                 throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
//             }

//             membership.Status = "paused";
//             membership.StartDate = null;
//             await _membershipRepository.UpdateAsync(membership.Id, membership);

//             return await GetMembershipByIdAsync(membershipId);
//         }

//         /// <summary>
//         /// Kích hoạt lại membership
//         /// </summary>
//         public async Task<MembershipDto> ResumeMembershipAsync(string membershipId)
//         {
//             var membership = await _membershipRepository.GetByIdAsync(membershipId);
//             if (membership == null)
//             {
//                 throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
//             }

//             if (membership.Status != "paused")
//             {
//                 throw new Exception("Gói tập này không ở trạng thái tạm dừng");
//             }

//             membership.Status = "active";
//             membership.StartDate = DateTime.UtcNow;
//             await _membershipRepository.UpdateAsync(membership.Id, membership);

//             return await GetMembershipByIdAsync(membershipId);
//         }

//         /// <summary>
//         /// Lấy thông tin chi tiết membership (giống getMembershipDetails của Express)
//         /// </summary>
//         public async Task<MembershipDetailsResponse> GetMembershipDetailsAsync(string memberId)
//         {
//             // 1. Lấy thông tin member
//             var member = await _memberRepository.GetByIdAsync(memberId);
//             if (member == null)
//             {
//                 throw new Exception("Không tìm thấy hội viên");
//             }

//             // 2. Lấy memberships (active hoặc expired)
//             var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
//             var validMemberships = memberships
//                 .Where(m => m.Status == "active" || m.Status == "expired")
//                 .ToList();

//             // 3. Nếu không có membership nào
//             if (!validMemberships.Any())
//             {
//                 return new MembershipDetailsResponse
//                 {
//                     MembershipId = "null",
//                     MemberName = member.Name,
//                     MemberAvatar = member.Avatar ?? "/placeholder-avatar.jpg",
//                     PackageId = "",
//                     PackageName = "Chưa đăng ký",
//                     PackageCategory = "basic",
//                     Status = "null",
//                     DaysRemaining = 0,
//                     SessionsRemaining = 0,
//                     TotalSessions = 0
//                 };
//             }

//             // 4. Tìm membership active trước, không có thì lấy expired
//             var selectedMembership = validMemberships.FirstOrDefault(m => m.Status == "active")
//                                    ?? validMemberships.FirstOrDefault(m => m.Status == "expired")
//                                    ?? validMemberships.First();

//             // 5. Tìm membership có giá cao nhất
//             var packages = new List<Domain.Entities.Package>();
//             foreach (var m in validMemberships)
//             {
//                 var pkg = await _packageRepository.GetByIdAsync(m.PackageId);
//                 if (pkg != null) packages.Add(pkg);
//             }

//             var highestPricePackage = packages.OrderByDescending(p => p.Price).FirstOrDefault();
//             var highestPriceMembership = validMemberships.FirstOrDefault(m => 
//                 m.PackageId == highestPricePackage?.Id) ?? selectedMembership;

//             // 6. Tính số ngày còn lại
//             int daysRemaining = 0;
//             if (highestPriceMembership.EndDate.HasValue)
//             {
//                 var diff = (highestPriceMembership.EndDate.Value - DateTime.UtcNow).TotalDays;
//                 daysRemaining = Math.Max(0, (int)Math.Ceiling(diff));
//             }

//             // 7. Tính số buổi tập còn lại
//             var sessionsRemaining = highestPriceMembership.AvailableSessions - highestPriceMembership.UsedSessions;

//             // 8. Lấy thông tin package
//             var packageInfo = await _packageRepository.GetByIdAsync(selectedMembership.PackageId);

//             return new MembershipDetailsResponse
//             {
//                 MembershipId = selectedMembership.Id,
//                 MemberName = member.Name,
//                 MemberAvatar = member.Avatar ?? "/placeholder-avatar.jpg",
//                 PackageId = packageInfo?.Id ?? "",
//                 PackageName = packageInfo?.Name ?? "Không có thông tin",
//                 PackageCategory = packageInfo?.Category ?? "basic",
//                 Status = selectedMembership.Status,
//                 DaysRemaining = daysRemaining,
//                 SessionsRemaining = sessionsRemaining,
//                 TotalSessions = highestPriceMembership.AvailableSessions
//             };
//         }

//         /// <summary>
//         /// Tự động cập nhật memberships hết hạn
//         /// </summary>
//         public async Task UpdateExpiredMembershipsAsync()
//         {
//             try
//             {
//                 var allMemberships = await _membershipRepository.GetAllAsync();
//                 var today = DateTime.UtcNow;
//                 int updatedCount = 0;

//                 foreach (var membership in allMemberships)
//                 {
//                     if (membership.Status == "active" && 
//                         membership.EndDate.HasValue && 
//                         membership.EndDate.Value < today)
//                     {
//                         membership.Status = "expired";
//                         await _membershipRepository.UpdateAsync(membership.Id, membership);
//                         updatedCount++;
//                     }
//                 }

//                 Console.WriteLine($"Đã cập nhật {updatedCount} membership hết hạn.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Lỗi khi cập nhật membership hết hạn: {ex.Message}");
//             }
//         }
//     }
// }