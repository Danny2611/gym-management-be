using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.Mappings.User;
using System.Security.Claims;
using GymManagement.Application.Services.User;
using GymManagement.Application.DTOs.User.Progress;

namespace GymManagement.API.Controllers.User
{
    [Authorize]
    [ApiController]
    [Route("api/user/progress")]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet("metrics/latest")]
        public async Task<IActionResult> GetLatestBodyMetrics()
        {
            try
            {
                var memberId = GetUserId();
                var metrics = await _progressService.GetLatestBodyMetricsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chỉ số cơ thể mới nhất thành công",
                    data = metrics?.ToDto()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("metrics/initial")]
        public async Task<IActionResult> GetInitialBodyMetrics()
        {
            try
            {
                var memberId = GetUserId();
                var metrics = await _progressService.GetInitialBodyMetricsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chỉ số cơ thể ban đầu thành công",
                    data = metrics?.ToDto()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("metrics/previous-month")]
        public async Task<IActionResult> GetPreviousMonthBodyMetrics()
        {
            try
            {
                var memberId = GetUserId();
                var metrics = await _progressService.GetPreviousMonthBodyMetricsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chỉ số cơ thể tháng trước thành công",
                    data = metrics?.ToDto()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("metrics/comparison")]
        public async Task<IActionResult> GetBodyMetricsComparison()
        {
            try
            {
                var memberId = GetUserId();
                var comparison = await _progressService.GetBodyMetricsComparisonAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy so sánh chỉ số cơ thể thành công",
                    data = comparison
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("metrics")]
        public async Task<IActionResult> UpdateBodyMetrics([FromBody] UpdateBodyMetricsDto dto)
        {
            try
            {
                var memberId = GetUserId();

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var metrics = await _progressService.UpdateBodyMetricsAsync(dto, memberId);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Cập nhật chỉ số cơ thể thành công",
                    data = metrics.ToDto()
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
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("stats/monthly")]
        public async Task<IActionResult> GetBodyStatsProgressByMonth([FromQuery] int months = 6)
        {
            try
            {
                var memberId = GetUserId();

                if (months <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Số tháng không hợp lệ"
                    });
                }

                var stats = await _progressService.GetBodyStatsProgressByMonthAsync(memberId, months);

                return Ok(new
                {
                    success = true,
                    message = "Lấy tiến độ chỉ số cơ thể theo tháng thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("radar")]
        public async Task<IActionResult> GetFitnessRadarData()
        {
            try
            {
                var memberId = GetUserId();
                var radarData = await _progressService.GetFitnessRadarDataAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu radar thể lực thành công",
                    data = radarData
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("metrics/changes")]
        public async Task<IActionResult> CalculateBodyMetricsChange()
        {
            try
            {
                var memberId = GetUserId();
                var changes = await _progressService.CalculateBodyMetricsChangeAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Tính toán thay đổi chỉ số cơ thể thành công",
                    data = changes
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("monthly-body-metrics")]
        public async Task<IActionResult> GetFormattedMonthlyBodyMetrics()
        {
            try
            {
                var memberId = GetUserId();
                var metrics = await _progressService.GetFormattedMonthlyBodyMetricsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chỉ số cơ thể mới nhất thành công",
                    data = metrics
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}