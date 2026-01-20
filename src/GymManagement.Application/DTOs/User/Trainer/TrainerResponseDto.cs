using System.Text.Json.Serialization;


namespace GymManagement.Application.DTOs.User.Trainer
{
    public class TrainerResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("specialization")]
        public string Specialization { get; set; }

        [JsonPropertyName("experience")]
        public int Experience { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("schedule")]
        public List<TrainerScheduleDto> Schedule { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class TrainerScheduleDto
    {
        [JsonPropertyName("day_of_week")]
        public int DayOfWeek { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }

        [JsonPropertyName("working_hours")]
        public List<WorkingHourDto> WorkingHours { get; set; }
    }

    public class WorkingHourDto
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

}
