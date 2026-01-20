using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings.Admin;

namespace GymManagement.Application.Services.Admin
{
    public class AdminPaymentService : IAdminPaymentService
    {
        private readonly IAdminPaymentRepository _paymentRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPackageRepository _packageRepository;

        public AdminPaymentService(
            IAdminPaymentRepository paymentRepository,
            IMemberRepository memberRepository,
            IPackageRepository packageRepository)
        {
            _paymentRepository = paymentRepository;
            _memberRepository = memberRepository;
            _packageRepository = packageRepository;
        }

        public async Task<PaymentListResponseDto> GetAllPaymentsAsync(PaymentQueryOptions options)
        {
            var (payments, totalCount) = await _paymentRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            var paymentDtos = new List<PaymentResponseDto>();

            foreach (var payment in payments)
            {
                var member = await _memberRepository.GetByIdAsync(payment.MemberId);
                var package = await _packageRepository.GetByIdAsync(payment.PackageId);

                // Apply search filter if provided
                if (!string.IsNullOrEmpty(options.Search))
                {
                    var searchLower = options.Search.ToLower();
                    var memberName = member?.Name?.ToLower() ?? "";
                    var memberEmail = member?.Email?.ToLower() ?? "";
                    var packageName = package?.Name?.ToLower() ?? "";
                    var transactionId = payment.TransactionId?.ToLower() ?? "";

                    if (!memberName.Contains(searchLower) &&
                        !memberEmail.Contains(searchLower) &&
                        !packageName.Contains(searchLower) &&
                        !transactionId.Contains(searchLower))
                    {
                        continue;
                    }
                }

                if (member != null && package != null)
                {
                    paymentDtos.Add(payment.ToDto(member, package));
                }
            }

            return new PaymentListResponseDto
            {
                Payments = paymentDtos,
                TotalPayments = paymentDtos.Count > 0 ? totalCount : 0,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<PaymentResponseDto> GetPaymentByIdAsync(string id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thanh toán");
            }

            var member = await _memberRepository.GetByIdAsync(payment.MemberId);
            var package = await _packageRepository.GetByIdAsync(payment.PackageId);

            if (member == null || package == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin thành viên hoặc gói dịch vụ");
            }

            return payment.ToDto(member, package);
        }

        public async Task<PaymentResponseDto> UpdatePaymentStatusAsync(
            string id,
            string status,
            string? transactionId)
        {
            var validStatuses = new[] { "pending", "completed", "failed", "cancelled" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Trạng thái thanh toán không hợp lệ");
            }

            var payment = await _paymentRepository.UpdateStatusAsync(id, status, transactionId);

            if (payment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thanh toán");
            }

            var member = await _memberRepository.GetByIdAsync(payment.MemberId);
            var package = await _packageRepository.GetByIdAsync(payment.PackageId);

            if (member == null || package == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin thành viên hoặc gói dịch vụ");
            }

            return payment.ToDto(member, package);
        }

        public async Task<PaymentListResponseDto> GetPaymentsByMemberIdAsync(
            string memberId,
            PaymentQueryOptions options)
        {
            var (payments, totalCount) = await _paymentRepository.GetByMemberIdAsync(memberId, options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            var paymentDtos = new List<PaymentResponseDto>();

            foreach (var payment in payments)
            {
                var member = await _memberRepository.GetByIdAsync(payment.MemberId);
                var package = await _packageRepository.GetByIdAsync(payment.PackageId);

                if (member != null && package != null)
                {
                    paymentDtos.Add(payment.ToDto(member, package));
                }
            }

            return new PaymentListResponseDto
            {
                Payments = paymentDtos,
                TotalPayments = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<PaymentStatisticsDto> GetPaymentStatisticsAsync()
        {
            return await _paymentRepository.GetStatisticsAsync();
        }
    }
}