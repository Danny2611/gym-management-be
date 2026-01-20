using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/packages")]
    public class AdminPackageController : ControllerBase
    {
        private readonly IAdminPackageService _packageService;
        private readonly ILogger<AdminPackageController> _logger;

        public AdminPackageController(
            IAdminPackageService packageService,
            ILogger<AdminPackageController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPackageStats()
        {
            try
            {
                var stats = await _packageService.GetPackageStatsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê gói dịch vụ thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê gói dịch vụ"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string search = null,
            [FromQuery] string status = null,
            [FromQuery] string category = null,
            [FromQuery] bool? popular = null,
            [FromQuery] string sortBy = null,
            [FromQuery] string sortOrder = "asc")
        {
            try
            {
                var options = new PackageQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    Category = category,
                    Popular = popular,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var result = await _packageService.GetAllPackagesAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách gói dịch vụ thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách gói dịch vụ"
                });
            }
        }

        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetPackageById(string packageId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID gói dịch vụ không hợp lệ"
                    });
                }

                var package = await _packageService.GetPackageByIdAsync(packageId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin gói dịch vụ thành công",
                    data = package
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thông tin gói dịch vụ"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var package = await _packageService.CreatePackageAsync(dto);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Tạo gói dịch vụ mới thành công",
                    data = package
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo gói dịch vụ mới");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi tạo gói dịch vụ mới"
                });
            }
        }

        [HttpPut("{packageId}")]
        public async Task<IActionResult> UpdatePackage(
            string packageId,
            [FromBody] UpdatePackageDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(packageId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID gói dịch vụ không hợp lệ"
                    });
                }

                var package = await _packageService.UpdatePackageAsync(packageId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật gói dịch vụ thành công",
                    data = package
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi cập nhật gói dịch vụ"
                });
            }
        }

        [HttpDelete("{packageId}")]
        public async Task<IActionResult> DeletePackage(string packageId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID gói dịch vụ không hợp lệ"
                    });
                }

                await _packageService.DeletePackageAsync(packageId);

                return Ok(new
                {
                    success = true,
                    message = "Xóa gói dịch vụ thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xóa gói dịch vụ"
                });
            }
        }

        [HttpPatch("{packageId}/status")]
        public async Task<IActionResult> TogglePackageStatus(string packageId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID gói dịch vụ không hợp lệ"
                    });
                }

                var package = await _packageService.TogglePackageStatusAsync(packageId);

                var statusMessage = package.Status == "active" ? "kích hoạt" : "vô hiệu hóa";

                return Ok(new
                {
                    success = true,
                    message = $"Gói dịch vụ đã được {statusMessage}",
                    data = package
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi trạng thái gói dịch vụ");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi thay đổi trạng thái gói dịch vụ"
                });
            }
        }
    }
}