using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Appointment
{
    // Request tạo appointment
    public class CreateAppointmentRequest
    {
        [JsonPropertyName("trainer_id")]
        public string TrainerId { get; set; }

        [JsonPropertyName("membership_id")]
        public string MembershipId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public string EndTime { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    // Request reschedule
    public class RescheduleAppointmentRequest
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public string EndTime { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    // Request check availability
    public class CheckAvailabilityRequest
    {
        [JsonPropertyName("trainer_id")]
        public string TrainerId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }
    }

    // Response appointment detail
    public class AppointmentDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("trainer")]
        public TrainerBasicDto Trainer { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; } // YYYY-MM-DD

        [JsonPropertyName("time")]
        public string Time { get; set; } // "HH:mm - HH:mm"

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }
    }

    public class TrainerBasicDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("specialization")]
        public string Specialization { get; set; }
    }

    // Response schedule item
    public class ScheduleDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; } // YYYY-MM-DD

        [JsonPropertyName("time")]
        public TimeSlotDto Time { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("trainer_id")]
        public string TrainerId { get; set; }

        [JsonPropertyName("trainer_name")]
        public string TrainerName { get; set; }

        [JsonPropertyName("trainer_image")]
        public string TrainerImage { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } // upcoming, completed, missed
    }

    public class TimeSlotDto
    {
        [JsonPropertyName("start_time")]
        public string StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public string EndTime { get; set; }
    }

    // Response upcoming appointment
    public class UpcomingAppointmentDto
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("timeStart")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("timeEnd")]
        public DateTime TimeEnd { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    // Response check availability
    // public class AvailabilityResponse
    // {
    //    // [JsonPropertyName("available")]
    //     public bool Available { get; set; }

    //   //  [JsonPropertyName("message")]
    //     public string Message { get; set; }
    // }

    public class AppointmentFilterDto
    {
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("search_term")]
        public string? SearchTerm { get; set; }

        [JsonPropertyName("time_slot")]
        public string? TimeSlot { get; set; }
    }

}