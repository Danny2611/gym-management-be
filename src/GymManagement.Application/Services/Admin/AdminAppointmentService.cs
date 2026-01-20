using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings.Admin;

namespace GymManagement.Application.Services.Admin
{
    public class AdminAppointmentService : IAdminAppointmentService
    {
        private readonly IAdminAppointmentRepository _appointmentRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ITrainerRepository _trainerRepository;

        public AdminAppointmentService(
            IAdminAppointmentRepository appointmentRepository,
            IMemberRepository memberRepository,
            ITrainerRepository trainerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _memberRepository = memberRepository;
            _trainerRepository = trainerRepository;
        }

        public async Task<AppointmentListResponseDto> GetAllAppointmentsAsync(
            AppointmentQueryOptions options)
        {
            var (appointments, totalCount) = await _appointmentRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            var appointmentDtos = new List<AppointmentResponseDto>();

            foreach (var appointment in appointments)
            {
                var member = await _memberRepository.GetByIdAsync(appointment.MemberId);
                var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);

                // Apply search filter if provided (search by member or trainer name)
                if (!string.IsNullOrEmpty(options.Search))
                {
                    var searchLower = options.Search.ToLower();
                    var memberName = member?.Name?.ToLower() ?? "";
                    var trainerName = trainer?.Name?.ToLower() ?? "";

                    if (!memberName.Contains(searchLower) && !trainerName.Contains(searchLower))
                    {
                        continue;
                    }
                }

                if (member != null && trainer != null)
                {
                    appointmentDtos.Add(appointment.ToDto(member, trainer));
                }
            }

            return new AppointmentListResponseDto
            {
                Appointments = appointmentDtos,
                TotalAppointments = appointmentDtos.Count > 0 ? totalCount : 0,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(string appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch hẹn");
            }

            var member = await _memberRepository.GetByIdAsync(appointment.MemberId);
            var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);

            if (member == null || trainer == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin hội viên hoặc huấn luyện viên");
            }

            return appointment.ToDto(member, trainer);
        }

        public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(
            string appointmentId,
            string status)
        {
            var validStatuses = new[] { "confirmed", "pending", "cancelled", "missed" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Trạng thái không hợp lệ");
            }

            var appointment = await _appointmentRepository.UpdateStatusAsync(appointmentId, status);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch hẹn");
            }

            var member = await _memberRepository.GetByIdAsync(appointment.MemberId);
            var trainer = await _trainerRepository.GetByIdAsync(appointment.TrainerId);

            if (member == null || trainer == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin hội viên hoặc huấn luyện viên");
            }

            return appointment.ToDto(member, trainer);
        }

        public async Task<AppointmentStatsDto> GetAppointmentStatsAsync()
        {
            return await _appointmentRepository.GetStatsAsync();
        }
    }
}