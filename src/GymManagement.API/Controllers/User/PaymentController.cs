using System.Text.Json;
using GymManagement.Application.DTOs.User.Payment;
using GymManagement.Application.Interfaces.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/user/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IPaymentService paymentService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo thanh toán MoMo
        /// POST /api/user/payment/momo
        /// </summary>
        [Authorize]
        [HttpPost("momo/create")]
        public async Task<IActionResult> CreateMoMoPayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để thực hiện thanh toán"
                    });
                }

                var response = await _paymentService.CreateMoMoPaymentAsync(userId, request);

                return Ok(new
                {
                    success = true,
                    message = "Đã tạo yêu cầu thanh toán",
                    data = response
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo thanh toán: {ex.Message}");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// MoMo IPN Callback (webhook từ MoMo)
        /// POST /api/user/payment/momo/ipn
        /// </summary>
        [HttpPost("momo/ipn")]

        public async Task<IActionResult> MoMoIpnCallback()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            Console.WriteLine("RAW IPN BODY:");
            Console.WriteLine(rawBody);

            MoMoIpnCallbackDto callbackData;

            try
            {
                callbackData = JsonSerializer.Deserialize<MoMoIpnCallbackDto>(
                    rawBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Deserialize IPN failed: {ex.Message}");
                return Ok(); // luôn trả 200 cho MoMo
            }

            if (callbackData == null)
            {
                Console.WriteLine("❌ IPN payload is null");
                return Ok();
            }

            try
            {
                await _paymentService.ProcessMoMoIpnCallbackAsync(callbackData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ IPN processing error: {ex.Message}");
            }

            // 🚨 BẮT BUỘC trả 200
            return Ok();
        }


        /// <summary>
        /// MoMo Redirect Callback (redirect từ MoMo về trang xác nhận)
        /// GET /api/user/payment/momo/callback
        /// </summary>
        [HttpGet("momo/callback")]
        public IActionResult MoMoRedirectCallback(
            [FromQuery] string orderId,
            [FromQuery] int resultCode)
        {
            try
            {
                var frontendUrl = _configuration["FrontendUrlMOMO"]
                                  ?? "http://localhost:5173";

                if (resultCode == 0)
                {
                    // Redirect về trang thành công
                    return Redirect(
                        $"{frontendUrl}/user/payment/success?orderId={orderId}&resultCode={resultCode}"
                    );
                }

                // Redirect về trang thất bại
                return Redirect(
                    $"{frontendUrl}/user/payment/failed?orderId={orderId}&resultCode={resultCode}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling MoMo redirect: {ex.Message}");

                var frontendUrl = _configuration["FrontendUrl"]
                                  ?? "http://localhost:5173";

                return Redirect(
                    $"{frontendUrl}/user/payment/failed?error=server_error"
                );
            }
        }

        /// <summary>
        /// Lấy trạng thái thanh toán
        /// GET /api/user/payment/{paymentId}/status
        /// </summary>
        [Authorize]
        [HttpGet("{paymentId}/status")]
        public async Task<IActionResult> GetPaymentStatus(string paymentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var userRole = User.FindFirst("role")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem trạng thái thanh toán"
                    });
                }

                var response = await _paymentService.GetPaymentStatusAsync(userId, paymentId, userRole);

                return Ok(new
                {
                    success = true,
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin thanh toán theo ID
        /// GET /api/user/payment/{paymentId}
        /// </summary>
        [Authorize]
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var userRole = User.FindFirst("role")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Bạn cần đăng nhập để xem thông tin thanh toán"
                    });
                }

                var payment = await _paymentService.GetPaymentByIdAsync(userId, paymentId, userRole);

                return Ok(new
                {
                    success = true,
                    data = payment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}