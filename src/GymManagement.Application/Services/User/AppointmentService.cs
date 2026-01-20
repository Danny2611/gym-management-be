using GymManagement.Application.DTOs.User.Appointment;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITrainerRepository _trainerRepository;
        private readonly IPackageRepository _packageRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IMembershipRepository membershipRepository,
            ITrainerRepository trainerRepository,
            IPackageRepository packageRepository)
        {
            _appointmentRepository = appointmentRepository;
            _membershipRepository = membershipRepository;
            _trainerRepository = trainerRepository;
            _packageRepository = packageRepository;
        }

        /// <summary>
        /// Tạo appointment mới
        /// </summary>
        public async Task<AppointmentDto> CreateAppointmentAsync(string memberId, CreateAppointmentRequest request)
        {
            // 1. Kiểm tra và cập nhật số buổi tập
            var membership = await CheckAndUpdateAvailableSessionsAsync(request.MembershipId);
            if (membership == null)
            {
                throw new Exception("Gói tập đã hết số lượt đăng ký với PT");
            }

            // 2. Kiểm tra trainer có lịch trống không
            var isAvailable = await IsTrainerAvailableAsync(
                request.TrainerId,
                request.Date,
                request.StartTime,
                request.EndTime);

            if (!isAvailable)
            {
                throw new Exception("Huấn luyện viên không có lịch trống vào thời gian này");
            }

            // 3. Tạo appointment
            var appointment = new Appointment
            {
                MemberId = memberId,
                TrainerId = request.TrainerId,
                MembershipId = request.MembershipId,
                Date = request.Date,
                Time = new AppointmentTime
                {
                    Start = request.StartTime,
                    End = request.EndTime
                },
                Location = request.Location ?? "Phòng Tập Chính",
                Notes = request.Notes ?? "",
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.CreateAsync(appointment);

            // 4. Cập nhật workingHours của trainer
            var trainer = await _trainerRepository.GetByIdAsync(request.TrainerId);
            if (trainer?.Schedule != null)
            {
                var dayOfWeek = (int)request.Date.DayOfWeek;
                var scheduleForDay = trainer.Schedule.FirstOrDefault(s =>
                    s.DayOfWeek == dayOfWeek && s.Available);

                if (scheduleForDay?.WorkingHours != null)
                {
                    var targetSlot = scheduleForDay.WorkingHours.FirstOrDefault(wh =>
                        wh.Start == request.StartTime && wh.End == request.EndTime);

                    if (targetSlot != null)
                    {
                        targetSlot.Available = false;
                        await _trainerRepository.UpdateAsync(trainer.Id, trainer);
                    }
                }
            }

            // 5. Cập nhật membership
            membership.AvailableSessions -= 1;
            membership.UsedSessions += 1;
            await _membershipRepository.UpdateAsync(membership.Id, membership);

            // 6. Lấy thông tin trainer để trả về
            var trainerInfo = await _trainerRepository.GetByIdAsync(request.TrainerId);

            return new AppointmentDto
            {
                Id = appointment.Id,
                Trainer = new TrainerBasicDto
                {
                    Id = trainerInfo?.Id ?? "",
                    Name = trainerInfo?.Name ?? "",
                    Image = trainerInfo?.Image ?? "",
                    Specialization = trainerInfo?.Specialization ?? ""
                },
                Date = appointment.Date.ToString("yyyy-MM-dd"),
                Time = $"{appointment.Time.Start} - {appointment.Time.End}",
                Location = appointment.Location,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        /// <summary>
        /// Lấy tất cả appointments của member
        /// </summary>
        public async Task<List<AppointmentDto>> GetAllMemberAppointmentsAsync(
            string memberId,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            string? searchTerm)
        {
            if (startDate.HasValue)
                startDate = startDate.Value.Date;

            if (endDate.HasValue)
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
            var appointments = await _appointmentRepository.GetByMemberIdWithFiltersAsync(
                memberId,
                startDate,
                endDate,
                status,
                searchTerm);

            var result = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);
                var membership = await _membershipRepository.GetByIdAsync(appointment.MembershipId);
                var package = membership != null
                    ? await _packageRepository.GetByIdAsync(membership.PackageId)
                    : null;
                result.Add(new AppointmentDto
                {
                    Id = appointment.Id,
                    Trainer = new TrainerBasicDto
                    {
                        Id = trainer?.Id ?? "",
                        Name = trainer?.Name ?? "",
                        Image = trainer?.Image ?? "",
                        Specialization = trainer?.Specialization ?? ""
                    },
                    Date = appointment.Date.ToString("yyyy-MM-dd"),
                    Time = $"{appointment.Time.Start} - {appointment.Time.End}",
                    Location = appointment.Location,
                    Status = appointment.Status,
                    Notes = appointment.Notes,
                    PackageName = package?.Name ?? "Gói tập"
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy appointment theo ID
        /// </summary>
        public async Task<AppointmentDto> GetAppointmentByIdAsync(string appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Không tìm thấy lịch hẹn");
            }

            var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);
            var membership = await _membershipRepository.GetByIdAsync(appointment.MembershipId);
            var package = membership != null
                ? await _packageRepository.GetByIdAsync(membership.PackageId)
                : null;
            return new AppointmentDto
            {
                Id = appointment.Id,
                Trainer = new TrainerBasicDto
                {
                    Id = trainer?.Id ?? "",
                    Name = trainer?.Name ?? "",
                    Image = trainer?.Image ?? "",
                    Specialization = trainer?.Specialization ?? ""
                },
                Date = appointment.Date.ToString("yyyy-MM-dd"),
                Time = $"{appointment.Time.Start} - {appointment.Time.End}",
                Location = appointment.Location,
                Status = appointment.Status,
                Notes = appointment.Notes,
                PackageName = package?.Name ?? "Gói tập"
            };
        }


        /// <summary>
        /// Lấy lịch tập (schedule) của member
        /// </summary>
        public async Task<List<ScheduleDto>> GetMemberScheduleAsync(
            string memberId,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            string? timeSlot,
            string? searchTerm)
        {
            var appointments = await _appointmentRepository.GetByMemberIdWithFiltersAsync(
                memberId,
                startDate,
                endDate,
                status ?? "confirmed",
                searchTerm);

            var result = new List<ScheduleDto>();

            foreach (var appointment in appointments)
            {
                // Get package name
                var membership = await _membershipRepository.GetByIdAsync(appointment.MembershipId);
                var package = membership != null
                    ? await _packageRepository.GetByIdAsync(membership.PackageId)
                    : null;

                var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);

                // Determine display status
                var displayStatus = GetDisplayStatus(appointment);

                // Filter by time slot if provided
                if (!string.IsNullOrEmpty(timeSlot))
                {
                    var hour = int.Parse(appointment.Time.Start.Split(':')[0]);
                    var appointmentTimeSlot = GetTimeSlotForHour(hour);
                    if (timeSlot != appointmentTimeSlot)
                        continue;
                }

                result.Add(new ScheduleDto
                {
                    Id = appointment.Id,
                    Date = appointment.Date.ToString("yyyy-MM-dd"),
                    Time = new TimeSlotDto
                    {
                        StartTime = appointment.Time.Start,
                        EndTime = appointment.Time.End
                    },
                    Location = appointment.Location,
                    Notes = appointment.Notes,
                    PackageName = package?.Name ?? "Gói tập",
                    TrainerId = trainer?.Id ?? "",
                    TrainerName = trainer?.Name ?? "",
                    TrainerImage = trainer?.Image ?? "",
                    Status = displayStatus
                });
            }

            return result;
        }

        /// <summary>
        /// Hủy appointment
        /// </summary>
        public async Task<AppointmentDto> CancelAppointmentAsync(string appointmentId, string memberId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Không tìm thấy lịch hẹn");
            }

            if (appointment.MemberId != memberId)
            {
                throw new Exception("Bạn không có quyền hủy lịch hẹn này");
            }

            // Kiểm tra điều kiện hủy
            var now = DateTime.UtcNow;
            var appointmentDate = appointment.Date;
            var oneDayInMs = TimeSpan.FromDays(1);
            var isAtLeastOneDayBefore = (appointmentDate - now) >= oneDayInMs;

            var canCancel = (appointment.Status == "pending" || appointment.Status == "confirmed")
                         && isAtLeastOneDayBefore;

            if (!canCancel)
            {
                throw new Exception("Bạn chỉ có thể hủy lịch hẹn ở trạng thái chờ xác nhận hoặc đã xác nhận, và trước ngày hẹn ít nhất 1 ngày");
            }

            // Cập nhật trạng thái
            appointment.Status = "cancelled";
            await _appointmentRepository.UpdateAsync(appointment.Id, appointment);

            // Hoàn trả buổi tập
            var membership = await _membershipRepository.GetByIdAsync(appointment.MembershipId);
            if (membership != null)
            {
                membership.AvailableSessions += 1;
                membership.UsedSessions -= 1;
                await _membershipRepository.UpdateAsync(membership.Id, membership);
            }

            return await GetAppointmentByIdAsync(appointmentId);
        }

        /// <summary>
        /// Reschedule appointment
        /// </summary>
        public async Task<AppointmentDto> RescheduleAppointmentAsync(
            string appointmentId,
            string memberId,
            RescheduleAppointmentRequest request)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Không tìm thấy lịch hẹn");
            }

            if (appointment.MemberId != memberId)
            {
                throw new Exception("Bạn không có quyền đổi lịch hẹn này");
            }

            // Kiểm tra điều kiện đổi lịch
            if (appointment.Status == "completed")
            {
                throw new Exception("Không thể đổi lịch hẹn đã hoàn thành");
            }

            var now = DateTime.UtcNow;
            var isAtLeastOneDayBefore = (appointment.Date - now) >= TimeSpan.FromDays(1);

            if (appointment.Status != "pending" && !isAtLeastOneDayBefore)
            {
                throw new Exception("Bạn chỉ có thể đổi lịch hẹn ở trạng thái chờ xác nhận hoặc trước ngày hẹn ít nhất 1 ngày");
            }

            // Kiểm tra trainer có lịch trống không
            var isAvailable = await IsTrainerAvailableAsync(
                appointment.TrainerId,
                request.Date,
                request.StartTime,
                request.EndTime);

            if (!isAvailable)
            {
                throw new Exception("Huấn luyện viên không có lịch trống vào thời gian này");
            }

            // Cập nhật thông tin
            appointment.Date = request.Date;
            appointment.Time = new AppointmentTime
            {
                Start = request.StartTime,
                End = request.EndTime
            };

            if (!string.IsNullOrEmpty(request.Location))
                appointment.Location = request.Location;

            if (!string.IsNullOrEmpty(request.Notes))
                appointment.Notes = request.Notes;

            await _appointmentRepository.UpdateAsync(appointment.Id, appointment);

            return await GetAppointmentByIdAsync(appointmentId);
        }

        /// <summary>
        /// Check trainer availability
        /// </summary>
        // public async Task<AvailabilityResponse> CheckTrainerAvailabilityAsync(CheckAvailabilityRequest request)
        // {
        //     var isAvailable = await IsTrainerAvailableAsync(
        //         request.TrainerId,
        //         request.Date,
        //         request.StartTime,
        //         request.EndTime);

        //     return new AvailabilityResponse
        //     {
        //         Available = isAvailable,
        //         Message = isAvailable 
        //             ? "Huấn luyện viên có lịch trống" 
        //             : "Huấn luyện viên không có lịch trống vào thời gian này"
        //     };
        // }

        /// <summary>
        /// Get upcoming appointments (next 7 days)
        /// </summary>
        public async Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync(string memberId)
        {
            var today = DateTime.UtcNow.Date;
            var nextWeek = today.AddDays(7);

            var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(
                memberId,
                today,
                nextWeek);

            return appointments.Select(app =>
            {
                var startParts = app.Time.Start.Split(':');
                var endParts = app.Time.End.Split(':');

                var timeStart = app.Date
                    .AddHours(int.Parse(startParts[0]))
                    .AddMinutes(int.Parse(startParts[1]));

                var timeEnd = app.Date
                    .AddHours(int.Parse(endParts[0]))
                    .AddMinutes(int.Parse(endParts[1]));

                return new UpcomingAppointmentDto
                {
                    Date = app.Date,
                    TimeStart = timeStart,
                    TimeEnd = timeEnd,
                    Location = app.Location,
                    Status = app.Status == "confirmed" ? "upcoming" : app.Status
                };
            }).ToList();
        }

        /// <summary>
        /// Update missed appointments (background job)
        /// </summary>
        public async Task UpdateMissedAppointmentsAsync()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            await _appointmentRepository.UpdateMissedAppointmentsAsync(yesterday);
            Console.WriteLine("Updated missed appointments");
        }

        // === PRIVATE HELPER METHODS ===

        private async Task<bool> IsTrainerAvailableAsync(
            string trainerId,
            DateTime date,
            string startTime,
            string endTime)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId);
            if (trainer == null) return false;

            // Kiểm tra schedule
            var dayOfWeek = (int)date.DayOfWeek;
            var scheduleForDay = trainer.Schedule?.FirstOrDefault(s =>
                s.DayOfWeek == dayOfWeek && s.Available);

            if (scheduleForDay == null) return false;

            // Kiểm tra working hours
            var isTimeSlotAvailable = scheduleForDay.WorkingHours?.Any(wh =>
                string.Compare(startTime, wh.Start) >= 0 &&
                string.Compare(endTime, wh.End) <= 0) ?? false;

            if (!isTimeSlotAvailable) return false;

            // Kiểm tra conflicting appointments
            var conflicts = await _appointmentRepository.GetConflictingAppointmentsAsync(
                trainerId,
                date,
                startTime,
                endTime);

            return conflicts.Count == 0;
        }

        private async Task<Membership?> CheckAndUpdateAvailableSessionsAsync(string membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null) return null;

            if (membership.Status != "active")
                throw new Exception("Membership is not active");

            // Check if need to reset monthly sessions
            var now = DateTime.UtcNow;
            var lastResetDate = membership.LastSessionsReset == default
                                    ? membership.StartDate
                                    : membership.LastSessionsReset;

            var thisMonth = new DateTime(now.Year, now.Month, 1);

            if (lastResetDate.HasValue && thisMonth > lastResetDate.Value)
            {
                var package = await _packageRepository.GetByIdAsync(membership.PackageId);
                membership.AvailableSessions = package?.TrainingSessions ?? 0;
                membership.LastSessionsReset = now;
                await _membershipRepository.UpdateAsync(membership.Id, membership);
            }

            if (membership.AvailableSessions <= 0)
                throw new Exception("No available training sessions left in this membership");

            return membership;
        }

        private string GetDisplayStatus(Appointment appointment)
        {
            if (appointment.Status == "cancelled") return "missed";

            var now = DateTime.UtcNow;
            var endParts = appointment.Time.End.Split(':');
            var appointmentEndTime = appointment.Date
                .AddHours(int.Parse(endParts[0]))
                .AddMinutes(int.Parse(endParts[1]));

            return appointmentEndTime < now ? "completed" : "upcoming";
        }

        private string GetTimeSlotForHour(int hour)
        {
            if (hour >= 6 && hour < 12) return "Sáng (6:00-12:00)";
            if (hour >= 12 && hour < 15) return "Trưa (12:00-15:00)";
            if (hour >= 15 && hour < 18) return "Chiều (15:00-18:00)";
            if (hour >= 18 && hour < 22) return "Tối (18:00-22:00)";
            return "Khác";
        }


    }
}

