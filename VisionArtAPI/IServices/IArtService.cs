using VisionArtAPI.Models;

namespace VisionArtAPI.IServices;

public interface IArtService
{
    Task<(byte[] bytes, string contentType, string fileName)> GetImageAsync(int id);
    Task<IEnumerable<ArtWork>> GetAllArtAsync();
    Task<IEnumerable<ArtWork>> SearchByTitleAsync(string title);
    Task<IEnumerable<ArtWork>> SearchByArtistAsync(string artist);
    Task<IEnumerable<ArtWork>> FilterByCategoryAsync(string category);
}

