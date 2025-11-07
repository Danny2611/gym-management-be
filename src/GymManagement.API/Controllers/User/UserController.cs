using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // GET: api/user/getuser
        [HttpGet("getuser")]
        public IActionResult GetUser()
        {
            var response = new
            {
                success = true,
                message = "User retrieved successfully",
                data = new
                {
                    name = "Trung",
                    role = "Member",
                    timestamp = DateTime.Now
                }
            };

            return Ok(response);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var response = new
            {
                success = true,
                message = $"User with ID {id} retrieved",
                data = new
                {
                    id = id,
                    name = "Trung",
                    email = "trung@example.com"
                }
            };

            return Ok(response);
        }

        // POST: api/user
        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            var response = new
            {
                success = true,
                message = "User created successfully",
                data = new
                {
                    id = 1,
                    name = request.Name,
                    email = request.Email
                }
            };

            return Created("api/user/1", response);
        }
    }

    // DTO cho request
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}