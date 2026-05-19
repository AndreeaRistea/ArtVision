namespace VisionArt.Web.Client.DTOs;

public class PaginatedArtworkResult
{
    public List<ArtWorkDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
