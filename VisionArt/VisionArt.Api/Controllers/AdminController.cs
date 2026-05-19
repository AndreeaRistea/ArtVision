using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionArt.Api.IServices;

namespace VisionArt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IDataImportService _importService;

    public AdminController(IDataImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("sync-database")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _importService.SyncMetadataAsync();
            return Ok(new { message = "Sync completed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
