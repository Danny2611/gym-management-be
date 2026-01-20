using GymManagement.Application.Mappings.User;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/public/trainers")]
public class TrainersController : ControllerBase
{
    private readonly ITrainerService _trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrainers()
    {
        var trainers = await _trainerService.GetTrainersAsync();

        return Ok(new
        {
            success = true,
            data = trainers.Select(t => t.ToDto())
        });
    }

    [HttpGet("{trainerId}")]
    public async Task<IActionResult> GetTrainerById(string trainerId)
    {
        var trainer = await _trainerService.GetTrainerByIdAsync(trainerId);

        if (trainer == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy huấn luyện viên"
            });
        }

        return Ok(new
        {
            success = true,
            data = trainer.ToDto()
        });
    }
}
