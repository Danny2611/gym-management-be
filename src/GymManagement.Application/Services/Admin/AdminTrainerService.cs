using System.Text.RegularExpressions;
using GymManagement.Application.DTOs.Admin.Trainer;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings;

namespace GymManagement.Application.Services.Admin
{
    public class AdminTrainerService : IAdminTrainerService
    {
        private readonly IAdminTrainerRepository _repository;

        public AdminTrainerService(IAdminTrainerRepository repository)
        {
            _repository = repository;
        }

        public async Task<TrainerListResponseDto> GetAllTrainersAsync(TrainerQueryOptions options)
        {
            var (trainers, totalCount) = await _repository.GetAllAsync(options);
            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            return new TrainerListResponseDto
            {
                Trainers = trainers.Select(t => t.ToDto()).ToList(),
                TotalTrainers = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<TrainerResponseDto> GetTrainerByIdAsync(string trainerId)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null) throw new KeyNotFoundException("Không tìm thấy huấn luyện viên");
            return trainer.ToDto();
        }

        public async Task<TrainerResponseDto> CreateTrainerAsync(CreateTrainerDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Tên và email là bắt buộc");

            var existing = await _repository.GetByEmailAsync(dto.Email);
            if (existing != null) throw new InvalidOperationException("Email đã tồn tại trong hệ thống");

            var trainer = dto.ToEntity();
            var created = await _repository.CreateAsync(trainer);
            return created.ToDto();
        }

        public async Task<TrainerResponseDto> UpdateTrainerAsync(string trainerId, UpdateTrainerDto dto)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null) throw new KeyNotFoundException("Không tìm thấy huấn luyện viên");

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != trainer.Email)
            {
                var existing = await _repository.GetByEmailAsync(dto.Email);
                if (existing != null) throw new InvalidOperationException("Email đã tồn tại trong hệ thống");
                trainer.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.Name)) trainer.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Image)) trainer.Image = dto.Image;
            if (!string.IsNullOrEmpty(dto.Bio)) trainer.Bio = dto.Bio;
            if (!string.IsNullOrEmpty(dto.Specialization)) trainer.Specialization = dto.Specialization;
            if (dto.Experience.HasValue) trainer.Experience = dto.Experience.Value;
            if (!string.IsNullOrEmpty(dto.Phone)) trainer.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Status)) trainer.Status = dto.Status;

            var updated = await _repository.UpdateAsync(trainerId, trainer);
            return updated.ToDto();
        }

        public async Task<TrainerResponseDto> UpdateTrainerScheduleAsync(string trainerId, List<TrainerScheduleDto> schedule)
        {
            if (schedule == null || schedule.Count != 7)
                throw new ArgumentException("Lịch làm việc phải có đầy đủ 7 ngày trong tuần");

            foreach (var day in schedule)
            {
                if (day.DayOfWeek < 0 || day.DayOfWeek > 6)
                    throw new ArgumentException("Ngày trong tuần phải từ 0 (Chủ nhật) đến 6 (Thứ 7)");

                if (day.Available && (day.WorkingHours == null || !day.WorkingHours.Any()))
                    throw new ArgumentException("Phải cung cấp thời gian làm việc cho ngày có trạng thái khả dụng");

                if (day.WorkingHours != null)
                {
                    var timeRegex = new Regex(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
                    foreach (var hours in day.WorkingHours)
                    {
                        if (!timeRegex.IsMatch(hours.Start) || !timeRegex.IsMatch(hours.End))
                            throw new ArgumentException("Thời gian phải theo định dạng HH:MM");

                        var startTime = DateTime.Parse($"1970-01-01 {hours.Start}");
                        var endTime = DateTime.Parse($"1970-01-01 {hours.End}");
                        if (endTime <= startTime)
                            throw new ArgumentException("Thời gian kết thúc phải sau thời gian bắt đầu");
                    }
                }
            }

            var scheduleEntities = schedule.ToScheduleEntities();
            var updated = await _repository.UpdateScheduleAsync(trainerId, scheduleEntities);
            return updated.ToDto();
        }

        public async Task<bool> DeleteTrainerAsync(string trainerId)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null) throw new KeyNotFoundException("Không tìm thấy huấn luyện viên");
            return await _repository.SoftDeleteAsync(trainerId);
        }

        public async Task<TrainerResponseDto> ToggleTrainerStatusAsync(string trainerId)
        {
            var updated = await _repository.ToggleStatusAsync(trainerId);
            return updated.ToDto();
        }

        public async Task<TrainerAvailabilityDto> GetTrainerAvailabilityAsync(string trainerId, DateTime startDate, DateTime endDate)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null) throw new KeyNotFoundException("Không tìm thấy huấn luyện viên");

            var availableDates = new List<AvailableDateDto>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                var dayOfWeek = (int)currentDate.DayOfWeek;
                var daySchedule = trainer.Schedule?.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);

                if (daySchedule?.Available == true && daySchedule.WorkingHours?.Any() == true)
                {
                    availableDates.Add(new AvailableDateDto
                    {
                        Date = currentDate,
                        DayOfWeek = dayOfWeek,
                        WorkingHours = daySchedule.WorkingHours.Select(w => new WorkingHourDto
                        {
                            Start = w.Start,
                            End = w.End,
                            Available = w.Available
                        }).ToList()
                    });
                }
                currentDate = currentDate.AddDays(1);
            }

            return new TrainerAvailabilityDto
            {
                Trainer = trainer.ToDto(),
                AvailableDates = availableDates
            };
        }

        public async Task<TrainerStatsDto> GetTrainerStatsAsync() =>
            await _repository.GetStatsAsync();
    }
}