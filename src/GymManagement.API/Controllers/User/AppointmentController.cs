using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.User.Appointment;
using GymManagement.Application.Interfaces.Services.User;


namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Tạo appointment mới
        /// POST /api/user/appointments
        /// </summary>
        // [HttpPost("appointments")]
        // public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        // {
        //     try
        //     {
        //         var userId = User.FindFirst("userId")?.Value;
        //         if (string.IsNullOrEmpty(userId))
        //         {
        //             return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
        //         }

        //         var appointment = await _appointmentService.CreateAppointmentAsync(userId, request);

        //         return Ok(new
        //         {
        //             success = true,
        //             message = "Đã tạo lịch hẹn thành công",
        //             data = appointment
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { success = false, message = ex.Message });
        //     }
        // }

        /// <summary>
        /// Lấy tất cả appointments của member
        /// GET /api/user/appointments
        /// </summary>
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] AppointmentFilterDto filter)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var data = await _appointmentService.GetAllMemberAppointmentsAsync(
                    userId,
                    filter.Status,
                    filter.StartDate,
                    filter.EndDate,
                    filter.SearchTerm
                );

                return Ok(new
                {
                    success = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy appointment theo ID
        /// GET /api/user/appointments/{appointmentId}
        /// </summary>
        [HttpGet("appointments/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById(string appointmentId)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

                return Ok(new
                {
                    success = true,
                    data = appointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch tập (schedule) của member
        /// GET /api/user/my-schedule
        /// </summary>
        [HttpGet("my-schedule")]
        public async Task<IActionResult> GetMemberSchedule(
           [FromQuery] AppointmentFilterDto filter)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var schedule = await _appointmentService.GetMemberScheduleAsync(
                    userId,
                    filter.Status,
                    filter.StartDate,
                    filter.EndDate,
                    filter.SearchTerm,
                    filter.TimeSlot
                );

                Console.WriteLine($" Du lieu Schedule: {schedule}");

                return Ok(new
                {
                    success = true,
                    data = schedule
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Hủy appointment
        /// DELETE /api/user/{appointmentId}/cancel
        /// </summary>
        [HttpDelete("{appointmentId}/cancel")]
        public async Task<IActionResult> CancelAppointment(string appointmentId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var appointment = await _appointmentService.CancelAppointmentAsync(appointmentId, userId);

                return Ok(new
                {
                    success = true,
                    message = "Đã hủy lịch hẹn thành công",
                    data = appointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reschedule appointment
        /// PUT /api/user/{appointmentId}/reschedule
        /// </summary>
        [HttpPut("{appointmentId}/reschedule")]
        public async Task<IActionResult> RescheduleAppointment(
            string appointmentId,
            [FromBody] RescheduleAppointmentRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var appointment = await _appointmentService.RescheduleAppointmentAsync(
                    appointmentId, userId, request);

                return Ok(new
                {
                    success = true,
                    message = "Đã đổi lịch hẹn thành công",
                    data = appointment
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check trainer availability
        /// POST /api/user/appointments/check-availability
        /// </summary>
        [HttpPost("appointments/check-availability")]
        // public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityRequest request)
        // {
        //     try
        //     {
        //         var result = await _appointmentService.CheckTrainerAvailabilityAsync(request);

        //         return Ok(new
        //         {
        //             success = true,
        //             data = result
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { success = false, message = ex.Message });
        //     }
        // }

        /// <summary>
        /// Get upcoming appointments (next 7 days)
        /// GET /api/user/appointments/next-week
        /// </summary>
        [HttpGet("appointments/next-week")]
        public async Task<IActionResult> GetUpcomingAppointments()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = appointments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}