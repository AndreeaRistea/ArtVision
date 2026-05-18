using Microsoft.AspNetCore.Mvc;
using VisionArtAPI.IServices;

namespace VisionArtAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : Controller
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
            return Ok(new { message = "Sync is done!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}


