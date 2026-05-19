using System.Net.Http.Json;
using VisionArt.Web.Client.DTOs;
using VisionArt.Shared.DTOs;

namespace VisionArt.Web.Client.Services;

public class ArtApiService
{
    private readonly HttpClient _http;

    public ArtApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ArtWorkDto>> GetAllAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ArtWorkDto>>($"{_http.BaseAddress}art/all") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching artworks: {ex.Message}");
            return new List<ArtWorkDto>();
        }
    }

    public async Task<List<ArtWorkDto>> GetFeaturedAsync(int count = 6)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ArtWorkDto>>($"{_http.BaseAddress}art/featured/{count}") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching featured artworks: {ex.Message}");
            return new List<ArtWorkDto>();
        }
    }

    public async Task<PaginatedArtworkResult> GetPaginatedAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            return await _http.GetFromJsonAsync<PaginatedArtworkResult>(
                $"{_http.BaseAddress}art/all?page={page}&pageSize={pageSize}") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching paginated artworks: {ex.Message}");
            return new PaginatedArtworkResult();
        }
    }

    public string GetImageUrl(int id)
    {
        return $"{_http.BaseAddress}art/{id}/image";
    }

    public string GetImageByPathUrl(string path)
    {
        return $"{_http.BaseAddress}art/image-by-path?path={Uri.EscapeDataString(path)}";
    }

    public async Task<List<SimilarArtResultDto>> UploadAndSearchAsync(Stream fileStream, string fileName, string contentType, int topK = 10)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent(topK.ToString()), "topK");

            var response = await _http.PostAsync($"{_http.BaseAddress}art/similar", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SimilarArtResultDto>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching similar artworks: {ex.Message}");
            return new List<SimilarArtResultDto>();
        }
    }
}

