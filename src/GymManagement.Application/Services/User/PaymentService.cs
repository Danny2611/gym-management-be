using GymManagement.Application.DTOs.User.Payment;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Domain.Entities;
using MongoDB.Bson;

namespace GymManagement.Application.Services.User
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMoMoPaymentService _momoService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMembershipRepository membershipRepository,
            IPackageRepository packageRepository,
            IPromotionRepository promotionRepository,
            IMoMoPaymentService momoService)
        {
            _paymentRepository = paymentRepository;
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
            _promotionRepository = promotionRepository;
            _momoService = momoService;
        }

        /// <summary>
        /// Tạo thanh toán MoMo cho gói tập
        /// </summary>
        public async Task<PaymentCreatedResponse> CreateMoMoPaymentAsync(string memberId, CreatePaymentRequest request)
        {
            // 1. Lấy thông tin gói tập
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

            // 3. Xóa các membership pending/expired cũ
            await _membershipRepository.DeleteManyAsync(
                memberId,
                request.PackageId,
                new List<string> { "pending", "expired" }
            );

            // 4. Kiểm tra khuyến mãi
            var promotions = await _promotionRepository.GetAllActivePromotionsAsync();
            var promotion = promotions.FirstOrDefault(p =>
                p.ApplicablePackages.Contains(request.PackageId));

            decimal finalPrice = packageInfo.Price;
            AppliedPromotion? appliedPromotion = null;

            if (promotion != null)
            {
                var discountAmount = (packageInfo.Price * (decimal)promotion.Discount) / 100;
                finalPrice = Math.Round(packageInfo.Price - discountAmount);

                appliedPromotion = new AppliedPromotion
                {
                    PromotionId = promotion.Id,
                    Name = promotion.Name,
                    Discount = promotion.Discount,
                    OriginalPrice = packageInfo.Price,
                    DiscountedPrice = finalPrice
                };
            }

            // 5. Tạo yêu cầu thanh toán MoMo
            var orderInfo = $"Thanh toán gói {packageInfo.Name} - FittLife";
            var momoResponse = await _momoService.CreatePaymentRequestAsync(
                request.PackageId,
                memberId,
                (long)finalPrice,
                orderInfo
            );

            if (momoResponse.ResultCode != 0)
            {
                throw new Exception($"Không thể tạo yêu cầu thanh toán: {momoResponse.Message}");
            }

            // 6. Lưu thông tin thanh toán vào database
            var payment = new Payment
            {
                MemberId = memberId,
                PackageId = request.PackageId,
                Amount = finalPrice,
                //OriginalAmount = packageInfo.Price,
                Status = "pending",
                PaymentMethod = "undefined",
                TransactionId = momoResponse.OrderId,
                Promotion = appliedPromotion,
                PaymentInfo = new BsonDocument
                {
                    { "requestId", momoResponse.RequestId },
                    { "payUrl", momoResponse.PayUrl },
                    { "orderId", momoResponse.OrderId },
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentRepository.CreateAsync(payment);

            // 7. Tạo membership với trạng thái pending
            var membership = new Membership
            {
                MemberId = memberId,
                PackageId = request.PackageId,
                PaymentId = payment.Id,
                StartDate = null,
                EndDate = null,
                AutoRenew = false,
                Status = "pending",
                AvailableSessions = packageInfo.TrainingSessions,
                UsedSessions = 0,
                LastSessionsReset = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _membershipRepository.CreateAsync(membership);

            // 8. Trả về response
            return new PaymentCreatedResponse
            {
                PaymentId = payment.Id,
                PayUrl = momoResponse.PayUrl,
                Amount = momoResponse.Amount,
                OriginalAmount = packageInfo.Price,
                Discount = promotion?.Discount ?? 0,
                PromotionApplied = promotion != null,
                TransactionId = momoResponse.OrderId,
                ExpireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (10 * 60 * 1000) // 10 phút
            };
        }

        /// <summary>
        /// Xử lý IPN callback từ MoMo
        /// </summary>
        public async Task ProcessMoMoIpnCallbackAsync(MoMoIpnCallbackDto callbackData)
        {
            // 1. Xác thực signature
            var isValid = _momoService.VerifyCallback(callbackData);
            if (!isValid)
            {
                throw new Exception("Invalid signature");
            }

            // 2. Kiểm tra mã kết quả
            if (callbackData.ResultCode != 0)
            {
                // Cập nhật trạng thái thanh toán failed
                var failedPayment = await _paymentRepository.GetByTransactionIdAsync(callbackData.OrderId);
                if (failedPayment != null)
                {
                    failedPayment.Status = "failed";
                    await _paymentRepository.UpdateAsync(failedPayment.Id, failedPayment);
                }
                throw new Exception($"Payment failed with code: {callbackData.ResultCode}");
            }

            // 3. Giải mã extraData
            var extraData = _momoService.DecodeExtraData(callbackData.ExtraData);
            if (string.IsNullOrEmpty(extraData.PackageId) || string.IsNullOrEmpty(extraData.MemberId))
            {
                throw new Exception("Invalid extraData format");
            }

            // 4. Cập nhật payment
            var payment = await _paymentRepository.GetByTransactionIdAsync(callbackData.OrderId);
            if (payment == null)
            {
                Console.WriteLine($"❌ Payment not found for OrderId: {callbackData.OrderId}");
                throw new Exception("Payment not found");
            }

            Console.WriteLine($"✅ Found payment: {payment.Id}");

            payment.Status = "completed";
            payment.PaymentMethod = callbackData.PayType;

            // Lưu toàn bộ callback data vào PaymentInfo
            payment.PaymentInfo = new BsonDocument
            {
                { "partnerCode", callbackData.PartnerCode ?? "" },
                { "orderId", callbackData.OrderId ?? "" },
                { "requestId", callbackData.RequestId ?? "" },
                { "amount", callbackData.Amount },
                { "orderInfo", callbackData.OrderInfo ?? "" },
                { "orderType", callbackData.OrderType ?? "" },
                { "transId", callbackData.TransId  },
                { "resultCode", callbackData.ResultCode },
                { "message", callbackData.Message ?? "" },
                { "payType", callbackData.PayType ?? "" },
                { "responseTime", callbackData.ResponseTime },
                { "extraData", callbackData.ExtraData ?? "" },
                { "signature", callbackData.Signature ?? "" }
            };

            await _paymentRepository.UpdateAsync(payment.Id, payment);
            Console.WriteLine($"✅ Updated payment status to completed");

            // 5. Lấy thông tin package
            var packageInfo = await _packageRepository.GetByIdAsync(extraData.PackageId);
            if (packageInfo == null)
            {
                throw new Exception("Package not found");
            }

            // 6. Tính thời gian bắt đầu và kết thúc
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(packageInfo.Duration);

            // 7. Kiểm tra membership hiện tại
            var existingMembership = await _membershipRepository.GetByMemberAndPackageAsync(
                extraData.MemberId,
                extraData.PackageId,
                "pending"
            );

            if (existingMembership != null)
            {
                // Cập nhật membership hiện tại
                existingMembership.PaymentId = payment.Id;
                existingMembership.StartDate = startDate;
                existingMembership.EndDate = endDate;
                existingMembership.Status = "active";
                await _membershipRepository.UpdateAsync(existingMembership.Id, existingMembership);

                Console.WriteLine($"Updated existing membership: {existingMembership.Id}");
            }
            else
            {
                // Tạo membership mới
                var membership = new Membership
                {
                    MemberId = extraData.MemberId,
                    PackageId = extraData.PackageId,
                    PaymentId = payment.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "active",
                    AvailableSessions = packageInfo.TrainingSessions,
                    UsedSessions = 0,
                    LastSessionsReset = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _membershipRepository.CreateAsync(membership);

                Console.WriteLine($"Created new membership: {membership.Id}");
            }
        }

        /// <summary>
        /// Lấy trạng thái thanh toán
        /// </summary>
        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string memberId, string paymentId, string? userRole)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new Exception("Không tìm thấy thông tin thanh toán");
            }

            // Kiểm tra quyền
            if (payment.MemberId != memberId && userRole != "Admin")
            {
                throw new Exception("Bạn không có quyền xem thông tin thanh toán này");
            }

            MembershipInfoDto? membershipInfo = null;

            // Nếu thanh toán đã hoàn thành, lấy thông tin membership
            if (payment.Status == "completed")
            {
                var memberships = await _membershipRepository.GetByMemberIdAsync(memberId);
                var membership = memberships.FirstOrDefault(m => m.PaymentId == paymentId);

                if (membership != null)
                {
                    var package = await _packageRepository.GetByIdAsync(membership.PackageId);
                    membershipInfo = new MembershipInfoDto
                    {
                        Id = membership.Id,
                        StartDate = membership.StartDate ?? DateTime.MinValue,
                        EndDate = membership.EndDate ?? DateTime.MinValue,
                        Status = membership.Status,
                        Package = new PackageBasicDto
                        {
                            Id = package?.Id ?? "",
                            Name = package?.Name ?? "",
                            // Price = package?.Price ?? 0
                        }
                    };
                }
            }

            return new PaymentStatusResponse
            {
                PaymentId = payment.Id,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                Membership = membershipInfo
            };
        }

        /// <summary>
        /// Lấy thông tin thanh toán theo ID
        /// </summary>
        public async Task<Payment> GetPaymentByIdAsync(string memberId, string paymentId, string? userRole)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new Exception("Không tìm thấy thông tin thanh toán");
            }

            // Kiểm tra quyền
            if (payment.MemberId != memberId && userRole != "Admin")
            {
                throw new Exception("Bạn không có quyền xem thông tin thanh toán này");
            }

            return payment;
        }
    }
}