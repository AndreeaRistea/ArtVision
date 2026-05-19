using Microsoft.AspNetCore.Identity;

namespace VisionArt.Api.Models;

public class User : IdentityUser
{
    public string FullName { get; set; }
    // Relație cu operele favorite
    public List<FavoriteArt> Favorites { get; set; } = new();
}
