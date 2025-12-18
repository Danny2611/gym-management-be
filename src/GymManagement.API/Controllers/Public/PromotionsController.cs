using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/public/promotions")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPromotions()
    {
        var promotions = await _promotionService.GetActivePromotionsAsync();
        return Ok(new
        {
            success = true,
            message = "Lấy danh sách khuyến mãi thành công",
            data = promotions
        });
    }
}
