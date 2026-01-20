using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.User.Workout;
using System.Security.Claims;
using GymManagement.Application.Services.User;
using GymManagement.Application.Mappings.User;

namespace GymManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/workout")]
    public class WorkoutScheduleController : ControllerBase
    {
        private readonly IWorkoutScheduleService _workoutService;

        public WorkoutScheduleController(IWorkoutScheduleService workoutService)
        {
            _workoutService = workoutService;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpPost("schedules")]
        public async Task<IActionResult> CreateWorkoutSchedule([FromBody] CreateWorkoutScheduleDto dto)
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

                var schedule = await _workoutService.CreateWorkoutScheduleAsync(dto, memberId);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Tạo lịch tập cá nhân thành công",
                    data = schedule.ToDto()
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

        [HttpGet("schedules")]
        public async Task<IActionResult> GetMemberWorkoutSchedules(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            try
            {
                var memberId = GetUserId();

                var schedules = await _workoutService.GetMemberWorkoutSchedulesAsync(
                    memberId, startDate, endDate, status);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách lịch tập thành công",
                    data = schedules.Select(s => s.ToDto())
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

        [HttpGet("schedules/{scheduleId}")]
        public async Task<IActionResult> GetWorkoutScheduleById(string scheduleId)
        {
            try
            {
                var memberId = GetUserId();

                var schedule = await _workoutService.GetWorkoutScheduleByIdAsync(scheduleId, memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy chi tiết lịch tập thành công",
                    data = schedule.ToDto()
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin lịch tập"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = "Bạn không có quyền xem lịch tập này"
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

        [HttpPatch("schedules/{scheduleId}/status")]
        public async Task<IActionResult> UpdateWorkoutScheduleStatus(
            string scheduleId,
            [FromBody] UpdateWorkoutStatusDto dto)
        {
            try
            {
                var memberId = GetUserId();

                if (string.IsNullOrEmpty(dto.Status) ||
                    !new[] { "upcoming", "completed", "missed" }.Contains(dto.Status))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không hợp lệ"
                    });
                }

                var schedule = await _workoutService.UpdateWorkoutScheduleStatusAsync(
                    scheduleId, dto.Status, memberId);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trạng thái lịch tập thành công",
                    data = schedule.ToDto()
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin lịch tập"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = "Bạn không có quyền cập nhật lịch tập này"
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

        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyWorkoutStats([FromQuery] DateTime? startDate)
        {
            try
            {
                var memberId = GetUserId();

                var stats = await _workoutService.GetWeeklyWorkoutStatsAsync(memberId, startDate);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê tập luyện tuần thành công",
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

        [HttpGet("monthly-comparison")]
        public async Task<IActionResult> GetMonthComparisonStats()
        {
            try
            {
                var memberId = GetUserId();

                var stats = await _workoutService.GetMonthComparisonStatsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê so sánh tháng thành công",
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

        [HttpGet("last-7-days")]
        public async Task<IActionResult> GetLast7DaysWorkouts()
        {
            try
            {
                var memberId = GetUserId();

                var workouts = await _workoutService.GetLast7DaysWorkoutsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch tập 7 ngày gần đây thành công",
                    data = workouts
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

        [HttpGet("next-week")]
        public async Task<IActionResult> GetUpcomingWorkouts()
        {
            try
            {
                var memberId = GetUserId();

                var workouts = await _workoutService.GetUpcomingWorkoutsAsync(memberId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch tập tuần sau",
                    data = workouts
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