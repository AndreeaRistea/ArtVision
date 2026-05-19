using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VisionArt.Api.Configuration;
using VisionArt.Api.Context;
using VisionArt.Api.DTOs;
using VisionArt.Api.IServices;
using VisionArt.Api.Models;

namespace VisionArt.Api.Services;

public class DataImportService : IDataImportService
{
    private readonly ArtDbContext _context;
    private readonly string _jsonPath;

    public DataImportService(ArtDbContext context, IOptions<StorageOptions> storageOptions)
    {
        _context = context;
        _jsonPath = Path.Combine(storageOptions.Value.PythonProjectImagesPath, "metadata.json");
    }

    public async Task SyncMetadataAsync()
    {
        if (!File.Exists(_jsonPath))
        {
            throw new FileNotFoundException($"Metadata file doens t exist: {_jsonPath}");
        }

        string jsonContent = await File.ReadAllTextAsync(_jsonPath);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var metadataList = System.Text.Json.JsonSerializer.Deserialize<List<MetMetadataDto>>(jsonContent, options);

        if (metadataList == null || metadataList.Count == 0)
        {
            return;
        }

        foreach (var dto in metadataList)
        {
            var artWork = new ArtWork
            {
                MetObjectId = dto.ObjectId,
                Title = dto.Title ?? "Unknown Title",
                Artist = dto.Artist ?? "Unknown Artist",
                Date = dto.Date,
                Category = dto.Category,
                ImagePath = dto.FilePath ,
                Description = dto.Description ?? "Unknown",
            };

            _context.ArtWorks.Add(artWork);
        }

        await _context.SaveChangesAsync();
    }
}

