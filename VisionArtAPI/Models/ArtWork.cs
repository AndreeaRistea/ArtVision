namespace VisionArtAPI.Models;

public class ArtWork
{
    public int Id { get; set; }
    public int MetObjectId { get; set; } 
    public string Title { get; set; }
    public string? Artist { get; set; }
    public string Category { get; set; } 
    public string Date { get; set; }
    public string ImagePath { get; set; } 
    public string? Description { get; set; }
    public string? Period { get; set; }
    public string? Culture { get; set; }
}

