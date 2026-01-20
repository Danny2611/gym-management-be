using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/public/packages")]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackagesController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPackages()
    {
        var packages = await _packageService.GetPackagesAsync();
        return Ok(packages);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageById(string id)
    {
        var result = await _packageService.GetPackageByIdAsync(id);
        if (result == null)
            return NotFound(new { message = "Không tìm thấy gói tập" });

        return Ok(result);
    }
}
