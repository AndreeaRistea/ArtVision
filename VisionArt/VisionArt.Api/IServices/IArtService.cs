using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VisionArt.Shared.DTOs;
using VisionArt.Api.Models;

namespace VisionArt.Api.IServices;

public interface IArtService
{
    Task<(byte[] bytes, string contentType, string fileName)> GetImageAsync(int id);
    IQueryable<ArtWork> GetArtworksQuery();
    Task<IEnumerable<ArtWork>> GetAllArtAsync();
    Task<IEnumerable<ArtWork>> GetFeaturedArtworksAsync(int count);
    Task<IEnumerable<ArtWork>> SearchByTitleAsync(string title);
    Task<IEnumerable<ArtWork>> SearchByArtistAsync(string artist);
    Task<IEnumerable<ArtWork>> FilterByCategoryAsync(string category);
    Task<IEnumerable<SimilarArtResultDto>> SearchSimilarAsync(IFormFile image, int topK = 10);
    Task<(byte[] bytes, string contentType, string fileName)> GetImageByPathAsync(string relativePath);
}

