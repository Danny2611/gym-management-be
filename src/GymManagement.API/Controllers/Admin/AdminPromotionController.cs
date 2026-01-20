using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using MongoDB.Bson;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminPromotionController : ControllerBase
    {
        private readonly IAdminPromotionService _promotionService;
        private readonly ILogger<AdminPromotionController> _logger;

        public AdminPromotionController(
            IAdminPromotionService promotionService,
            ILogger<AdminPromotionController> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        // GET: api/admin/promotions/stats
        [HttpGet("promotions/stats")]
        public async Task<IActionResult> GetPromotionStats()
        {
            try
            {
                var stats = await _promotionService.GetPromotionStatsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê chương trình khuyến mãi thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê chương trình khuyến mãi");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê chương trình khuyến mãi"
                });
            }
        }

        // GET: api/admin/promotions
        [HttpGet("promotions")]
        public async Task<IActionResult> GetAllPromotions(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            try
            {
                var options = new PromotionQueryOptions
                {
                    Page = page,
                    Limit = limit,
                    Search = search,
                    Status = status,
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var promotionsData = await _promotionService.GetAllPromotionsAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách chương trình khuyến mãi thành công",
                    data = promotionsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách chương trình khuyến mãi");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách chương trình khuyến mãi"
                });
            }
        }

        // GET: api/admin/promotions/{id}
        [HttpGet("promotions/{id}")]
        public async Task<IActionResult> GetPromotionById(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID chương trình khuyến mãi không hợp lệ"
                    });
                }

                var promotion = await _promotionService.GetPromotionByIdAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin chương trình khuyến mãi thành công",
                    data = promotion
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy chương trình khuyến mãi: {PromotionId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chương trình khuyến mãi: {PromotionId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thông tin chương trình khuyến mãi"
                });
            }
        }

        // POST: api/admin/promotions
        [HttpPost("promotions")]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionDto request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên chương trình khuyến mãi không được để trống"
                    });
                }

                if (request.ApplicablePackages == null || !request.ApplicablePackages.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Danh sách gói áp dụng phải là mảng và không được rỗng"
                    });
                }

                // Validate ObjectIds in applicable_packages
                foreach (var packageId in request.ApplicablePackages)
                {
                    if (!ObjectId.TryParse(packageId, out _))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Một hoặc nhiều ID gói không hợp lệ"
                        });
                    }
                }

                var newPromotion = await _promotionService.CreatePromotionAsync(request);

                return StatusCode(201, new
                {
                    success = true,
                    message = "Tạo chương trình khuyến mãi thành công",
                    data = newPromotion
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
                _logger.LogError(ex, "Lỗi khi tạo chương trình khuyến mãi");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi tạo chương trình khuyến mãi"
                });
            }
        }

        // PUT: api/admin/promotions/{id}
        [HttpPut("promotions/{id}")]
        public async Task<IActionResult> UpdatePromotion(
            string id,
            [FromBody] UpdatePromotionDto request)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID chương trình khuyến mãi không hợp lệ"
                    });
                }

                // Validate applicable_packages if provided
                if (request.ApplicablePackages != null)
                {
                    if (!request.ApplicablePackages.Any())
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Danh sách gói áp dụng phải là mảng và không được rỗng"
                        });
                    }

                    foreach (var packageId in request.ApplicablePackages)
                    {
                        if (!ObjectId.TryParse(packageId, out _))
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = "Một hoặc nhiều ID gói không hợp lệ"
                            });
                        }
                    }
                }

                var updatedPromotion = await _promotionService.UpdatePromotionAsync(id, request);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật chương trình khuyến mãi thành công",
                    data = updatedPromotion
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy chương trình khuyến mãi: {PromotionId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
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
                _logger.LogError(ex, "Lỗi khi cập nhật chương trình khuyến mãi: {PromotionId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi cập nhật chương trình khuyến mãi"
                });
            }
        }

        // DELETE: api/admin/promotions/{id}
        [HttpDelete("promotions/{id}")]
        public async Task<IActionResult> DeletePromotion(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID chương trình khuyến mãi không hợp lệ"
                    });
                }

                await _promotionService.DeletePromotionAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Xóa chương trình khuyến mãi thành công"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy chương trình khuyến mãi: {PromotionId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa chương trình khuyến mãi: {PromotionId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xóa chương trình khuyến mãi"
                });
            }
        }

        // GET: api/admin/promotions/{id}/effectiveness
        [HttpGet("promotions/{id}/effectiveness")]
        public async Task<IActionResult> GetPromotionEffectiveness(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID chương trình khuyến mãi không hợp lệ"
                    });
                }

                var effectiveness = await _promotionService.GetPromotionEffectivenessAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Lấy báo cáo hiệu quả khuyến mãi thành công",
                    data = effectiveness
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy chương trình khuyến mãi: {PromotionId}", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo hiệu quả khuyến mãi: {PromotionId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy báo cáo hiệu quả khuyến mãi"
                });
            }
        }

        // GET: api/admin/packages/{packageId}/promotions/active
        [HttpGet("packages/{packageId}/promotions/active")]
        public async Task<IActionResult> GetActivePromotionsForPackage(string packageId)
        {
            try
            {
                if (!ObjectId.TryParse(packageId, out _))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID gói dịch vụ không hợp lệ"
                    });
                }

                var activePromotions = await _promotionService.GetActivePromotionsForPackageAsync(packageId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách khuyến mãi đang hoạt động cho gói thành công",
                    data = activePromotions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách khuyến mãi cho gói: {PackageId}", packageId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy danh sách khuyến mãi cho gói"
                });
            }
        }

        // PATCH: api/admin/promotions/statuses/update
        [HttpPatch("promotions/statuses/update")]
        public async Task<IActionResult> UpdatePromotionStatuses()
        {
            try
            {
                await _promotionService.UpdatePromotionStatusesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trạng thái chương trình khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái chương trình khuyến mãi");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi cập nhật trạng thái chương trình khuyến mãi"
                });
            }
        }
    }
}