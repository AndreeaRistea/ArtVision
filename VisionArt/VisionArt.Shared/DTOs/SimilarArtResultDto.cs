namespace VisionArt.Shared.DTOs;

public class SimilarArtResultDto
{
    public int Id { get; set; }
    public int MetObjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double SimilarityScore { get; set; }
}
