using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.DTOs;
using GymManagement.Application.DTOs.User.Responses;

namespace GymManagement.Application.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IPackageDetailRepository _packageDetailRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPaymentRepository _paymentRepository;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IPackageDetailRepository packageDetailRepository,
            IPackageRepository packageRepository,
            IPaymentRepository paymentRepository)
        {
            _membershipRepository = membershipRepository;
            _packageDetailRepository = packageDetailRepository;
            _packageRepository = packageRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<List<string>> GetMemberTrainingLocationsAsync(string memberId)
        {
            // Get all active memberships for the member
            var memberships = await _membershipRepository.GetActiveMembershipsByMemberIdAsync(memberId);

            if (memberships == null || memberships.Count == 0)
            {
                return new List<string>();
            }

            // Extract package IDs from memberships
            var packageIds = memberships.Select(m => m.PackageId).ToList();

            // Get all package details for these packages
            var packageDetails = await _packageDetailRepository.GetByPackageIdsAsync(packageIds);

            // Collect unique training areas
            var uniqueLocations = new HashSet<string>();

            foreach (var detail in packageDetails)
            {
                if (detail.TrainingAreas != null && detail.TrainingAreas.Count > 0)
                {
                    foreach (var location in detail.TrainingAreas)
                    {
                        uniqueLocations.Add(location);
                    }
                }
            }

            return uniqueLocations.ToList();
        }

        public async Task<List<MembershipResponse>> GetMemberMembershipsAsync(string memberId)
        {
            // Get all memberships for the member
            var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);

            if (memberships == null || memberships.Count == 0)
            {
                return new List<MembershipResponse>();
            }

            // Get unique package IDs and payment IDs
            var packageIds = memberships.Select(m => m.PackageId).Distinct().ToList();
            var paymentIds = memberships.Select(m => m.PaymentId).Distinct().ToList();

            // Fetch all packages and payments in parallel
            var packagesTask = _packageRepository.GetByIdsAsync(packageIds);
            var paymentsTask = _paymentRepository.GetByIdsAsync(paymentIds);

            await Task.WhenAll(packagesTask, paymentsTask);

            var packages = packagesTask.Result.ToDictionary(p => p.Id);
            var payments = paymentsTask.Result.ToDictionary(p => p.Id);

            // Map memberships to response DTOs
            var response = memberships
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MembershipResponse
                {
                    Id = m.Id,
                    MemberId = m.MemberId,
                    PackageId = packages.ContainsKey(m.PackageId) ? new PackageInfo
                    {
                        Id = packages[m.PackageId].Id,
                        Name = packages[m.PackageId].Name,
                        Price = packages[m.PackageId].Price,
                        Duration = packages[m.PackageId].Duration,
                        Description = packages[m.PackageId].Description,
                        Benefits = packages[m.PackageId].Benefits,
                        Status = packages[m.PackageId].Status,
                        Category = packages[m.PackageId].Category,
                        Popular = packages[m.PackageId].Popular,
                        TrainingSessions = packages[m.PackageId].TrainingSessions,
                        SessionDuration = packages[m.PackageId].SessionDuration
                    } : null,
                    PaymentId = payments.ContainsKey(m.PaymentId) ? new DTOs.User.Responses.PaymentInfo
                    {
                        Id = payments[m.PaymentId].Id,
                        Amount = payments[m.PaymentId].Amount,
                        Status = payments[m.PaymentId].Status,
                        PaymentMethod = payments[m.PaymentId].PaymentMethod,
                        TransactionId = payments[m.PaymentId].TransactionId,
                        CreatedAt = payments[m.PaymentId].CreatedAt
                    } : null,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    AutoRenew = m.AutoRenew,
                    Status = m.Status,
                    AvailableSessions = m.AvailableSessions,
                    UsedSessions = m.UsedSessions,
                    LastSessionsReset = m.LastSessionsReset,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToList();

            return response;
        }

        public async Task<List<MembershipResponse>> GetActiveMemberMembershipsAsync(string memberId)
        {
            // Get only active memberships for the member
            var memberships = await _membershipRepository.GetActiveMembershipsByMemberIdAsync(memberId);

            if (memberships == null || memberships.Count == 0)
            {
                return new List<MembershipResponse>();
            }

            // Get unique package IDs and payment IDs
            var packageIds = memberships.Select(m => m.PackageId).Distinct().ToList();
            var paymentIds = memberships.Select(m => m.PaymentId).Distinct().ToList();

            // Fetch all packages and payments in parallel
            var packagesTask = _packageRepository.GetByIdsAsync(packageIds);
            var paymentsTask = _paymentRepository.GetByIdsAsync(paymentIds);

            await Task.WhenAll(packagesTask, paymentsTask);

            var packages = packagesTask.Result.ToDictionary(p => p.Id);
            var payments = paymentsTask.Result.ToDictionary(p => p.Id);

            // Map memberships to response DTOs
            var response = memberships
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MembershipResponse
                {
                    Id = m.Id,
                    MemberId = m.MemberId,
                    PackageId = packages.ContainsKey(m.PackageId) ? new PackageInfo
                    {
                        Id = packages[m.PackageId].Id,
                        Name = packages[m.PackageId].Name,
                        Price = packages[m.PackageId].Price,
                        Duration = packages[m.PackageId].Duration,
                        Description = packages[m.PackageId].Description,
                        Benefits = packages[m.PackageId].Benefits,
                        Status = packages[m.PackageId].Status,
                        Category = packages[m.PackageId].Category,
                        Popular = packages[m.PackageId].Popular,
                        TrainingSessions = packages[m.PackageId].TrainingSessions,
                        SessionDuration = packages[m.PackageId].SessionDuration
                    } : null,
                    PaymentId = payments.ContainsKey(m.PaymentId) ? new DTOs.User.Responses.PaymentInfo
                    {
                        Id = payments[m.PaymentId].Id,
                        Amount = payments[m.PaymentId].Amount,
                        Status = payments[m.PaymentId].Status,
                        PaymentMethod = payments[m.PaymentId].PaymentMethod,
                        TransactionId = payments[m.PaymentId].TransactionId,
                        CreatedAt = payments[m.PaymentId].CreatedAt
                    } : null,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    AutoRenew = m.AutoRenew,
                    Status = m.Status,
                    AvailableSessions = m.AvailableSessions,
                    UsedSessions = m.UsedSessions,
                    LastSessionsReset = m.LastSessionsReset,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToList();

            return response;
        }

        public async Task<MembershipResponse> GetMembershipByIdAsync(string membershipId)
        {
            // Get the membership by ID
            var membership = await _membershipRepository.GetByIdAsync(membershipId);

            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            // Fetch package and payment details
            var packageTask = _packageRepository.GetByIdAsync(membership.PackageId);
            var paymentTask = _paymentRepository.GetByIdAsync(membership.PaymentId);

            await Task.WhenAll(packageTask, paymentTask);

            var package = packageTask.Result;
            var payment = paymentTask.Result;

            // Map to response DTO
            var response = new MembershipResponse
            {
                Id = membership.Id,
                MemberId = membership.MemberId,
                PackageId = package != null ? new PackageInfo
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    Description = package.Description,
                    Benefits = package.Benefits,
                    Status = package.Status,
                    Category = package.Category,
                    Popular = package.Popular,
                    TrainingSessions = package.TrainingSessions,
                    SessionDuration = package.SessionDuration
                } : null,
                PaymentId = payment != null ? new DTOs.User.Responses.PaymentInfo
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    CreatedAt = payment.CreatedAt
                } : null,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                AutoRenew = membership.AutoRenew,
                Status = membership.Status,
                AvailableSessions = membership.AvailableSessions,
                UsedSessions = membership.UsedSessions,
                LastSessionsReset = membership.LastSessionsReset,
                CreatedAt = membership.CreatedAt,
                UpdatedAt = membership.UpdatedAt
            };

            return response;
        }

        public async Task<MembershipResponse> PauseMembershipAsync(string membershipId)
        {
            // Get the membership by ID
            var membership = await _membershipRepository.GetByIdAsync(membershipId);

            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            // Update status to paused and clear start_date
            membership.Status = "paused";
            membership.StartDate = null;
            membership.UpdatedAt = DateTime.Now;

            // Save the updated membership
            await _membershipRepository.UpdateAsync(membershipId, membership);

            // Fetch package and payment details
            var packageTask = _packageRepository.GetByIdAsync(membership.PackageId);
            var paymentTask = _paymentRepository.GetByIdAsync(membership.PaymentId);

            await Task.WhenAll(packageTask, paymentTask);

            var package = packageTask.Result;
            var payment = paymentTask.Result;

            // Map to response DTO
            var response = new MembershipResponse
            {
                Id = membership.Id,
                MemberId = membership.MemberId,
                PackageId = package != null ? new PackageInfo
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    Description = package.Description,
                    Benefits = package.Benefits,
                    Status = package.Status,
                    Category = package.Category,
                    Popular = package.Popular,
                    TrainingSessions = package.TrainingSessions,
                    SessionDuration = package.SessionDuration
                } : null,
                PaymentId = payment != null ? new DTOs.User.Responses.PaymentInfo
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    CreatedAt = payment.CreatedAt
                } : null,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                AutoRenew = membership.AutoRenew,
                Status = membership.Status,
                AvailableSessions = membership.AvailableSessions,
                UsedSessions = membership.UsedSessions,
                LastSessionsReset = membership.LastSessionsReset,
                CreatedAt = membership.CreatedAt,
                UpdatedAt = membership.UpdatedAt
            };

            return response;
        }

        public async Task<MembershipResponse> ResumeMembershipAsync(string membershipId)
        {
            // Get the membership by ID
            var membership = await _membershipRepository.GetByIdAsync(membershipId);

            if (membership == null)
            {
                throw new Exception("Không tìm thấy thông tin gói tập đã đăng ký");
            }

            // Check if membership is paused
            if (membership.Status != "paused")
            {
                throw new Exception("Gói tập này không ở trạng thái tạm dừng");
            }

            // Update status to active and set start_date to now
            membership.Status = "active";
            membership.StartDate = DateTime.Now;
            membership.UpdatedAt = DateTime.Now;

            // Save the updated membership
            await _membershipRepository.UpdateAsync(membershipId, membership);

            // Fetch package and payment details
            var packageTask = _packageRepository.GetByIdAsync(membership.PackageId);
            var paymentTask = _paymentRepository.GetByIdAsync(membership.PaymentId);

            await Task.WhenAll(packageTask, paymentTask);

            var package = packageTask.Result;
            var payment = paymentTask.Result;

            // Map to response DTO
            var response = new MembershipResponse
            {
                Id = membership.Id,
                MemberId = membership.MemberId,
                PackageId = package != null ? new PackageInfo
                {
                    Id = package.Id,
                    Name = package.Name,
                    Price = package.Price,
                    Duration = package.Duration,
                    Description = package.Description,
                    Benefits = package.Benefits,
                    Status = package.Status,
                    Category = package.Category,
                    Popular = package.Popular,
                    TrainingSessions = package.TrainingSessions,
                    SessionDuration = package.SessionDuration
                } : null,
                PaymentId = payment != null ? new DTOs.User.Responses.PaymentInfo
                {
                    Id = payment.Id,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    CreatedAt = payment.CreatedAt
                } : null,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                AutoRenew = membership.AutoRenew,
                Status = membership.Status,
                AvailableSessions = membership.AvailableSessions,
                UsedSessions = membership.UsedSessions,
                LastSessionsReset = membership.LastSessionsReset,
                CreatedAt = membership.CreatedAt,
                UpdatedAt = membership.UpdatedAt
            };

            return response;
        }

        public async Task<MembershipDetailsResponse> GetMembershipDetailsAsync(string memberId)
        {
            // Get member from repository (we need IMemberRepository to fetch member details)
            // For now, we'll work with memberships directly
            var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);

            // If no memberships found, return default response
            if (memberships == null || memberships.Count == 0)
            {
                return new MembershipDetailsResponse
                {
                    MembershipId = "null",
                    MemberName = "Unknown",
                    MemberAvatar = "/placeholder-avatar.jpg",
                    PackageId = "",
                    PackageName = "Chưa đăng ký",
                    PackageCategory = "basic",
                    Status = "null",
                    DaysRemaining = 0,
                    SessionsRemaining = 0,
                    TotalSessions = 0
                };
            }

            // Filter memberships by status: prefer 'active' then 'expired'
            var activeMemberships = memberships.Where(m => m.Status == "active").ToList();
            var expiredMemberships = memberships.Where(m => m.Status == "expired").ToList();

            var selectedMembership = activeMemberships.FirstOrDefault() ?? expiredMemberships.FirstOrDefault() ?? memberships.FirstOrDefault();

            // Find membership with highest price package
            var packageIds = memberships.Select(m => m.PackageId).Distinct().ToList();
            var packages = await _packageRepository.GetByIdsAsync(packageIds);
            
            var highestPriceMembership = memberships.OrderByDescending(m =>
            {
                var pkg = packages.FirstOrDefault(p => p.Id == m.PackageId);
                return pkg?.Price ?? 0;
            }).FirstOrDefault();

            // Calculate days remaining
            int daysRemaining = 0;
            if (highestPriceMembership?.EndDate.HasValue == true)
            {
                var today = DateTime.Now;
                var endDate = highestPriceMembership.EndDate.Value;
                var timespan = endDate - today;
                daysRemaining = Math.Max(0, (int)Math.Ceiling(timespan.TotalDays));
            }

            // Calculate sessions remaining
            var sessionsRemaining = highestPriceMembership?.AvailableSessions - highestPriceMembership?.UsedSessions ?? 0;
            var totalSessions = highestPriceMembership?.AvailableSessions ?? 0;

            // Get package info for highest price membership
            var packageInfo = packages.FirstOrDefault(p => p.Id == highestPriceMembership?.PackageId);

            return new MembershipDetailsResponse
            {
                MembershipId = selectedMembership.Id,
                MemberName = memberId, // We'd need IMemberRepository to get actual member name
                MemberAvatar = "/placeholder-avatar.jpg", // We'd need IMemberRepository
                PackageId = packageInfo?.Id ?? "",
                PackageName = packageInfo?.Name ?? "Không có thông tin",
                PackageCategory = packageInfo?.Category ?? "basic",
                Status = selectedMembership?.Status ?? "unknown",
                DaysRemaining = daysRemaining,
                SessionsRemaining = (int)sessionsRemaining,
                TotalSessions = (int)totalSessions
            };
        }
    }
}
