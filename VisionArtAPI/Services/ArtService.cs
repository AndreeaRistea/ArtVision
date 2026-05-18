using Microsoft.EntityFrameworkCore;
using VisionArtAPI.Context;
using VisionArtAPI.IServices;
using VisionArtAPI.Models;

namespace VisionArtAPI.Services;

public class ArtService : IArtService

{
    private readonly ArtDbContext _context;
    private readonly string _baseStoragePath;

    public ArtService(ArtDbContext context, IConfiguration config)
    {
        _context = context;
        _baseStoragePath = config["StorageSettings:PythonProjectImagesPath"]
                           ?? throw new Exception("The pasth is not config");
    }
    public async Task<(byte[] bytes, string contentType, string fileName)> GetImageAsync(int id)
    {
        var art = await _context.ArtWorks.FindAsync(id);
        if (art == null || string.IsNullOrEmpty(art.ImagePath))
            throw new FileNotFoundException("The ArtWork is not found in database.");

        var fullPath = Path.Combine(_baseStoragePath, art.ImagePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"The physical file could not be found at the following path: {fullPath}");

        var bytes = await File.ReadAllBytesAsync(fullPath);

        var contentType = fullPath.EndsWith(".png") ? "image/png" : "image/jpeg";

        return (bytes, contentType, Path.GetFileName(fullPath));
    }

    public async Task<IEnumerable<ArtWork>> GetAllArtAsync()
    {
        return await _context.ArtWorks.ToListAsync();
    }

    public async Task<IEnumerable<ArtWork>> SearchByTitleAsync(string title)
    {
        return await _context.ArtWorks
            .Where(a => a.Title.Contains(title))
            .ToListAsync();
    }

    public async Task<IEnumerable<ArtWork>> SearchByArtistAsync(string artist)
    {
        return await _context.ArtWorks
            .Where(a => a.Artist.Contains(artist))
            .ToListAsync();
    }

    public async Task<IEnumerable<ArtWork>> FilterByCategoryAsync(string category)
    {
        return await _context.ArtWorks
            .Where(a => a.Category == category)
            .ToListAsync();
    }

}

