namespace VisionArtAPI.Models;

public class FavoriteArt
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ArtWorkId { get; set; }
    public ArtWork ArtWork { get; set; }
}

