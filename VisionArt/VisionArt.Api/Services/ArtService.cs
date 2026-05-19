using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VisionArt.Api.Configuration;
using VisionArt.Api.Context;
using VisionArt.Shared.DTOs;
using VisionArt.Api.IServices;
using VisionArt.Api.Models;

namespace VisionArt.Api.Services;

public class ArtService : IArtService

{
    private readonly ArtDbContext _context;
    private readonly string _baseStoragePath;
    private readonly HttpClient _httpClient;

    public ArtService(ArtDbContext context, IOptions<StorageOptions> storageOptions, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _baseStoragePath = storageOptions.Value.PythonProjectImagesPath;
        _httpClient = httpClientFactory.CreateClient("PythonApi");
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

    public IQueryable<ArtWork> GetArtworksQuery()
    {
        return _context.ArtWorks;
    }

    public async Task<IEnumerable<ArtWork>> GetAllArtAsync()
    {
        return await _context.ArtWorks.ToListAsync();
    }

    public async Task<IEnumerable<ArtWork>> GetFeaturedArtworksAsync(int count)
    {
        return await _context.ArtWorks.Take(count).ToListAsync();
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

    public async Task<IEnumerable<SimilarArtResultDto>> SearchSimilarAsync(IFormFile image, int topK = 10)
    {
        await using var imageStream = image.OpenReadStream();
        var fileContent = new StreamContent(imageStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);

        using var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "file", image.FileName);
        formData.Add(new StringContent(topK.ToString()), "top_k");

        var response = await _httpClient.PostAsync("/api/similar", formData);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<SimilarResponse>();

        if (json?.Results == null || json.Results.Count == 0)
            return Enumerable.Empty<SimilarArtResultDto>();

        var objectIds = json.Results
            .Select(r =>
            {
                var name = Path.GetFileNameWithoutExtension(r.FilePath?.Replace("/", "\\"));
                var parts = name.Split('_');
                return parts.Length >= 2 && int.TryParse(parts[^1], out var id) ? id : (int?) null;
            })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        var artworks = await _context.ArtWorks
            .Where(a => objectIds.Contains(a.MetObjectId))
            .ToListAsync();

        var pathToScore = json.Results
            .GroupBy(r => Path.GetFileNameWithoutExtension(r.FilePath?.Replace("/", "\\")))
            .ToDictionary(g => g.Key, g => g.First().SimilarityScore);

        return artworks.Select(a => new SimilarArtResultDto
        {
            Id = a.Id,
            MetObjectId = a.MetObjectId,
            Title = a.Title,
            Artist = a.Artist,
            Category = a.Category,
            Date = a.Date,
            ImagePath = a.ImagePath,
            Description = a.Description,
            SimilarityScore = pathToScore.GetValueOrDefault($"art_{a.MetObjectId}", 0)
        }).OrderByDescending(r => r.SimilarityScore);
    }

    public async Task<(byte[] bytes, string contentType, string fileName)> GetImageByPathAsync(string relativePath)
    {
        var normalizedPath = relativePath.Replace("/", "\\");
        var fullPath = Path.Combine(_baseStoragePath, normalizedPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Image not found at path: {fullPath}");

        var bytes = await File.ReadAllBytesAsync(fullPath);
        var contentType = fullPath.EndsWith(".png") ? "image/png" : "image/jpeg";
        return (bytes, contentType, Path.GetFileName(fullPath));
    }

    private class SimilarResponse
    {
        public List<SimilarItem> Results { get; set; } = new();
    }

    private class SimilarItem
    {
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("similarity_score")]
        public double SimilarityScore { get; set; }
    }
}