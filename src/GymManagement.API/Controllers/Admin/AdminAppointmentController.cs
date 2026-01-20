using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using MongoDB.Bson;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/appointments")]
    public class AdminAppointmentController : ControllerBase
    {
        private readonly IAdminAppointmentService _appointmentService;
        private readonly ILogger<AdminAppointmentController> _logger;

        public AdminAppointmentController(
            IAdminAppointmentService appointmentService,
            ILogger<AdminAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // GET: api/admin/appointments/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetAppointmentStats()
        {
            try
            {
                var stats = await _appointmentService.GetAppointmentStatsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê lịch hẹn thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê lịch hẹn");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê lịch hẹn",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/appointments
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? member_id = null,
            [FromQuery] string? trainer_id = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            try
            {
                var options = new AppointmentQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    StartDate = startDate,
                    EndDate = endDate,
                    MemberId = member_id,
                    TrainerId = trainer_id,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var appointmentsData = await _appointmentService.GetAllAppointmentsAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách lịch hẹn thành công",
                    data = appointmentsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lịch hẹn");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách lịch hẹn",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/appointments/{appointmentId}
        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(string appointmentId)
        {
            try
            {
                if (!ObjectId.TryParse(appointmentId, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID lịch hẹn không hợp lệ"
                    });
                }

                var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin lịch hẹn thành công",
                    data = appointment
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy lịch hẹn: {AppointmentId}", appointmentId);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin lịch hẹn: {AppointmentId}", appointmentId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thông tin lịch hẹn"
                });
            }
        }

        // PATCH: api/admin/appointments/{appointmentId}/status
        [HttpPatch("{appointmentId}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(
            string appointmentId,
            [FromBody] UpdateAppointmentStatusDto request)
        {
            try
            {
                if (!ObjectId.TryParse(appointmentId, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID lịch hẹn không hợp lệ"
                    });
                }

                if (string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không được để trống"
                    });
                }

                var updatedAppointment = await _appointmentService
                    .UpdateAppointmentStatusAsync(appointmentId, request.Status);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trạng thái lịch hẹn thành công",
                    data = updatedAppointment
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy lịch hẹn: {AppointmentId}", appointmentId);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái lịch hẹn: {AppointmentId}", appointmentId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi cập nhật trạng thái lịch hẹn"
                });
            }
        }
    }
}