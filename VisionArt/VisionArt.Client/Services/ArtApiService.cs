using System.Net.Http.Json;
using VisionArt.Client.DTOs;

namespace VisionArt.Client.Services;

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
            // HttpClient va folosi automat BaseAddress-ul configurat anterior
            return await _http.GetFromJsonAsync<List<ArtWorkDto>>($"{_http.BaseAddress}art/all") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching artworks: {ex.Message}");
            return new List<ArtWorkDto>();
        }
    }

    public string GetImageUrl(int id)
    {var url = $"{_http.BaseAddress}art/{id}/image";
        return url;
    }
}

