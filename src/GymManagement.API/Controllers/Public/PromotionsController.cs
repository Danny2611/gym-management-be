
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.API.Controllers.User
{
    [ApiController]
    [Route("api/public/promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        /// <summary>
        /// Lấy tất cả promotions đang active
        /// GET /api/promotions/active
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActivePromotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllActivePromotionsAsync();

                return Ok(new
                {
                    success = true,
                    data = promotions,
                    message = "Lấy danh sách khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả promotions (bao gồm inactive)
        /// GET /api/promotions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllPromotionsAsync();

                return Ok(new
                {
                    success = true,
                    data = promotions,
                    message = "Lấy danh sách khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy promotion theo ID
        /// GET /api/promotions/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotionById(string id)
        {
            try
            {
                var promotion = await _promotionService.GetByIdAsync(id);

                if (promotion == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy khuyến mãi"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = promotion,
                    message = "Lấy thông tin khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}