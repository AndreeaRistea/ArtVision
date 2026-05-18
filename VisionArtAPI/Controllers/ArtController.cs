using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisionArtAPI.IServices;
using VisionArtAPI.Services;

namespace VisionArtAPI.Controllersp;

[Route("api/[controller]")]
[ApiController]
public class ArtController : ControllerBase
{
    private readonly IArtService _artService;

    public ArtController(IArtService artService)
    {
        _artService = artService;
    }
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _artService.GetAllArtAsync());
    }

    [HttpGet("{id}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var (bytes, contentType, fileName) =
            await _artService.GetImageAsync(id);

        return File(bytes, contentType);
    }

    [HttpGet("search/title/{title}")]
    public async Task<IActionResult> GetByTitle(string title)
        => Ok(await _artService.SearchByTitleAsync(title));

    [HttpGet("search/artist/{artist}")]
    public async Task<IActionResult> GetByArtist(string artist)
        => Ok(await _artService.SearchByArtistAsync(artist));

    [HttpGet("filter/category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
        => Ok(await _artService.FilterByCategoryAsync(category));
}

