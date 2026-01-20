using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin.Trainer;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/trainers")]
    public class AdminTrainerController : ControllerBase
    {
        private readonly IAdminTrainerService _trainerService;
        private readonly ILogger<AdminTrainerController> _logger;

        public AdminTrainerController(IAdminTrainerService trainerService, ILogger<AdminTrainerController> logger)
        {
            _trainerService = trainerService;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetTrainerStats()
        {
            try
            {
                var stats = await _trainerService.GetTrainerStatsAsync();
                return Ok(new { success = true, message = "Lấy thống kê huấn luyện viên thành công", data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê huấn luyện viên");
                return StatusCode(500, new { success = false, message = "Lỗi server khi lấy thống kê huấn luyện viên", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrainers([FromQuery] int page = 1, [FromQuery] int limit = 10,
            [FromQuery] string search = null, [FromQuery] string status = null, [FromQuery] string specialization = null,
            [FromQuery] int? experience = null, [FromQuery] string sortBy = null, [FromQuery] string sortOrder = "asc")
        {
            try
            {
                var options = new TrainerQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    Specialization = specialization,
                    Experience = experience,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };
                var result = await _trainerService.GetAllTrainersAsync(options);
                return Ok(new { success = true, message = "Lấy danh sách huấn luyện viên thành công", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách huấn luyện viên");
                return StatusCode(500, new { success = false, message = "Lỗi server khi lấy danh sách huấn luyện viên", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrainerById(string id)
        {
            try
            {
                var trainer = await _trainerService.GetTrainerByIdAsync(id);
                return Ok(new { success = true, message = "Lấy thông tin huấn luyện viên thành công", data = trainer });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin huấn luyện viên");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainer([FromBody] CreateTrainerDto dto)
        {
            try
            {
                var trainer = await _trainerService.CreateTrainerAsync(dto);
                return StatusCode(201, new { success = true, message = "Tạo huấn luyện viên thành công", data = trainer });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo huấn luyện viên");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTrainer(string id, [FromBody] UpdateTrainerDto dto)
        {
            try
            {
                var trainer = await _trainerService.UpdateTrainerAsync(id, dto);
                return Ok(new { success = true, message = "Cập nhật huấn luyện viên thành công", data = trainer });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật huấn luyện viên");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(string id)
        {
            try
            {
                await _trainerService.DeleteTrainerAsync(id);
                return Ok(new { success = true, message = "Xóa huấn luyện viên thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa huấn luyện viên");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("{id}/schedule")]
        public async Task<IActionResult> UpdateTrainerSchedule(string id, [FromBody] UpdateScheduleDto dto)
        {
            try
            {
                var trainer = await _trainerService.UpdateTrainerScheduleAsync(id, dto.Schedule);
                return Ok(new { success = true, message = "Cập nhật lịch làm việc thành công", data = trainer });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lịch làm việc");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetTrainerAvailability(string id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                if (!startDate.HasValue || !endDate.HasValue)
                    return BadRequest(new { success = false, message = "Ngày bắt đầu và kết thúc là bắt buộc" });

                if (startDate.Value > endDate.Value)
                    return BadRequest(new { success = false, message = "Ngày bắt đầu phải trước ngày kết thúc" });

                var availability = await _trainerService.GetTrainerAvailabilityAsync(id, startDate.Value, endDate.Value);
                return Ok(new { success = true, message = "Lấy thông tin khả dụng của huấn luyện viên thành công", data = availability });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin khả dụng");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleTrainerStatus(string id)
        {
            try
            {
                var trainer = await _trainerService.ToggleTrainerStatusAsync(id);
                var statusMsg = trainer.Status == "active" ? "kích hoạt" : "vô hiệu hóa";
                return Ok(new { success = true, message = $"Trainer đã được {statusMsg}", data = trainer });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái trainer");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}