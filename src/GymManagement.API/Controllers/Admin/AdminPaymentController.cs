using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using MongoDB.Bson;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminPaymentController : ControllerBase
    {
        private readonly IAdminPaymentService _paymentService;
        private readonly ILogger<AdminPaymentController> _logger;

        public AdminPaymentController(
            IAdminPaymentService paymentService,
            ILogger<AdminPaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET: api/admin/payments/statistics
        [HttpGet("payments/statistics")]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            try
            {
                var statistics = await _paymentService.GetPaymentStatisticsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê thanh toán thành công",
                    data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê thanh toán");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê thanh toán"
                });
            }
        }

        // GET: api/admin/payments
        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPayments(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var options = new PaymentQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    PaymentMethod = paymentMethod,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var paymentsData = await _paymentService.GetAllPaymentsAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách thanh toán thành công",
                    data = paymentsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thanh toán");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách thanh toán"
                });
            }
        }

        // GET: api/admin/payments/{id}
        [HttpGet("payments/{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID thanh toán không hợp lệ"
                    });
                }

                var payment = await _paymentService.GetPaymentByIdAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin thanh toán theo ID thành công",
                    data = payment
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy thanh toán: {PaymentId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin thanh toán: {PaymentId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thông tin thanh toán"
                });
            }
        }

        // PATCH: api/admin/payments/{id}/status
        [HttpPatch("payments/{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(
            string id,
            [FromBody] UpdatePaymentStatusDto request)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID thanh toán không hợp lệ"
                    });
                }

                if (string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái thanh toán là bắt buộc"
                    });
                }

                var validStatuses = new[] { "pending", "completed", "failed", "cancelled" };
                if (!validStatuses.Contains(request.Status))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái thanh toán không hợp lệ"
                    });
                }

                var updatedPayment = await _paymentService.UpdatePaymentStatusAsync(
                    id,
                    request.Status,
                    request.TransactionId
                );

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thanh toán thành công",
                    data = updatedPayment
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy thanh toán: {PaymentId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái thanh toán: {PaymentId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi cập nhật trạng thái thanh toán"
                });
            }
        }

        // GET: api/admin/members/{memberId}/payments
        [HttpGet("members/{memberId}/payments")]
        public async Task<IActionResult> GetPaymentsByMemberId(
            string memberId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                if (!ObjectId.TryParse(memberId, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID thành viên không hợp lệ"
                    });
                }

                var options = new PaymentQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Status = status,
                    PaymentMethod = paymentMethod,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };

                var paymentsData = await _paymentService.GetPaymentsByMemberIdAsync(memberId, options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách thanh toán của thành viên thành công",
                    data = paymentsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thanh toán của thành viên: {MemberId}", memberId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách thanh toán của thành viên"
                });
            }
        }
    }
}