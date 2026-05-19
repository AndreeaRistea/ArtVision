using System.ComponentModel.DataAnnotations;

namespace VisionArt.Api.Configuration;

public class CorsOptions
{
    public const string SectionName = "CorsSettings";

    [Required]
    public string[] AllowedOrigins { get; set; } = [];
}
