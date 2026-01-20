using MongoDB.Bson;
using System.Text.Json;
using GymManagement.Application.DTOs.User.Transaction;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.User;

namespace GymManagement.Application.Services.User
{
    public class TransactionService : ITransactionService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IMemberRepository _memberRepository;

        public TransactionService(
            IPaymentRepository paymentRepository,
            IPackageRepository packageRepository,
            IMemberRepository memberRepository)
        {
            _paymentRepository = paymentRepository;
            _packageRepository = packageRepository;
            _memberRepository = memberRepository;
        }

        /// <summary>
        /// Lấy tất cả transactions của member với filters
        /// </summary>
        public async Task<List<TransactionItemDto>> GetAllMemberTransactionsAsync(
            string memberId,
            TransactionFilterDto filter)
        {
            var payments = await _paymentRepository.GetByMemberIdWithFiltersAsync(
                memberId,
                filter.Status,
                filter.PaymentMethod,
                filter.StartDate,
                filter.EndDate
            );

            var result = new List<TransactionItemDto>();

            foreach (var payment in payments)
            {
                var package = await _packageRepository.GetByIdAsync(payment.PackageId);

                result.Add(new TransactionItemDto
                {
                    Id = payment.Id,
                    PackageName = package?.Name ?? "Unknown Package",
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId ?? "N/A",
                    Date = payment.CreatedAt,
                    PaymentInfo = BsonDocumentToObject(payment.PaymentInfo)
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy chi tiết transaction theo ID
        /// </summary>
        public async Task<TransactionDetailDto> GetTransactionByIdAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByIdAsync(transactionId);
            if (payment == null)
            {
                throw new Exception("Transaction not found");
            }

            var member = await _memberRepository.GetByIdAsync(payment.MemberId);
            var package = await _packageRepository.GetByIdAsync(payment.PackageId);

            return new TransactionDetailDto
            {
                Id = payment.Id,
                MemberId = new MemberBasicDto
                {
                    Id = member?.Id ?? "",
                    Name = member?.Name ?? "",
                    Image = member?.Avatar ?? ""
                },
                PackageId = new PackageDetailDto
                {
                    Id = package?.Id ?? "",
                    Category = package?.Category ?? "",
                    Description = package?.Description ?? "",
                    Duration = package?.Duration ?? 0,
                    Name = package?.Name ?? "",
                    Price = package?.Price ?? 0
                },
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                PackageName = package?.Name ?? "Unknown Package",
                Amount = payment.Amount,
                TransactionId = payment.TransactionId ?? "N/A",
                PaymentInfo = BsonDocumentToObject(payment.PaymentInfo),
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }

        /// <summary>
        /// Lấy 5 transactions gần đây đã thành công
        /// </summary>
        public async Task<List<RecentTransactionDto>> GetRecentSuccessfulTransactionsAsync(string memberId)
        {
            var payments = await _paymentRepository.GetRecentSuccessfulByMemberIdAsync(memberId, 5);

            var result = new List<RecentTransactionDto>();

            foreach (var payment in payments)
            {
                var package = await _packageRepository.GetByIdAsync(payment.PackageId);

                result.Add(new RecentTransactionDto
                {
                    Amount = payment.Amount,
                    CreatedAt = payment.CreatedAt,
                    PackageName = package?.Name
                });
            }

            return result;
        }

        /// <summary>
        /// Convert BsonDocument to object for JSON serialization
        /// </summary>
        private Dictionary<string, object> BsonDocumentToObject(BsonDocument? bsonDoc)
        {
            if (bsonDoc == null)
                return new Dictionary<string, object>();

            var json = bsonDoc.ToJson();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                ?? new Dictionary<string, object>();
        }

    }
}