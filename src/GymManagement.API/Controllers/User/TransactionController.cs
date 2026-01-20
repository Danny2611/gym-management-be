using GymManagement.Application.DTOs.User.Transaction;
using GymManagement.Application.Interfaces.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Lấy danh sách giao dịch của member
        /// GET /api/user/transactions?status=completed&paymentMethod=qr&startDate=2025-01-01&endDate=2025-12-31
        /// </summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionFilterDto filter)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem lịch sử giao dịch"
                    });
                }

                var transactions = await _transactionService.GetAllMemberTransactionsAsync(userId, filter);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách giao dịch thành công",
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách giao dịch: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách giao dịch"
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết giao dịch
        /// POST /api/user/transaction-details
        /// Body: { "transactionId": "..." }
        /// </summary>
        [HttpPost("transaction-details")]
        public async Task<IActionResult> GetTransactionById([FromBody] GetTransactionRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem chi tiết giao dịch"
                    });
                }

                if (string.IsNullOrEmpty(request.TransactionId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu thông tin giao dịch cần xem"
                    });
                }

                var transaction = await _transactionService.GetTransactionByIdAsync(request.TransactionId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết giao dịch thành công",
                    data = transaction
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy chi tiết giao dịch: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy 5 giao dịch thành công gần đây
        /// GET /api/user/transaction/success
        /// </summary>
        [HttpGet("transaction/success")]
        public async Task<IActionResult> GetRecentSuccessfulTransactions()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem các giao dịch gần đây"
                    });
                }

                var transactions = await _transactionService.GetRecentSuccessfulTransactionsAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy các giao dịch gần đây",
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy các giao dịch gần đây: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy các giao dịch gần đây"
                });
            }
        }
    }
}