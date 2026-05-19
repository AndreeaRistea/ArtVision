using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisionArt.Api.DTOs;
using VisionArt.Api.IServices;

namespace VisionArt.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtController : ControllerBase
{
    private readonly IArtService _artService;

    public ArtController(IArtService artService)
    {
        _artService = artService;
    }

    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;

        var query = _artService.GetArtworksQuery();
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [AllowAnonymous]
    [HttpGet("featured/{count}")]
    public async Task<IActionResult> GetFeatured(int count)
    {
        if (count < 1) count = 6;
        if (count > 50) count = 50;
        return Ok(await _artService.GetFeaturedArtworksAsync(count));
    }

    [AllowAnonymous]
    [HttpGet("{id}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var (bytes, contentType, fileName) =
            await _artService.GetImageAsync(id);

        return File(bytes, contentType);
    }

    [AllowAnonymous]
    [HttpGet("search/title/{title}")]
    public async Task<IActionResult> GetByTitle(string title)
        => Ok(await _artService.SearchByTitleAsync(title));

    [AllowAnonymous]
    [HttpGet("search/artist/{artist}")]
    public async Task<IActionResult> GetByArtist(string artist)
        => Ok(await _artService.SearchByArtistAsync(artist));

    [AllowAnonymous]
    [HttpGet("filter/category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
        => Ok(await _artService.FilterByCategoryAsync(category));

    [AllowAnonymous]
    [HttpPost("similar")]
    public async Task<IActionResult> SearchSimilar(IFormFile file, [FromForm] int topK = 10)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded" });

        var results = await _artService.SearchSimilarAsync(file, topK);
        return Ok(results);
    }

    [AllowAnonymous]
    [HttpGet("image-by-path")]
    public async Task<IActionResult> GetImageByPath([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest(new { error = "Path is required" });

        try
        {
            var (bytes, contentType, fileName) = await _artService.GetImageByPathAsync(path);
            return File(bytes, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { error = "Image not found" });
        }
    }
}
