using System.Text.Json.Serialization;

namespace VisionArt.Client.DTOs;

public class ArtWorkDto
{
    public int Id { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

