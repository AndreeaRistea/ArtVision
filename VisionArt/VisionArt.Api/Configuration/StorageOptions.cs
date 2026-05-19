using System.ComponentModel.DataAnnotations;

namespace VisionArt.Api.Configuration;

public class StorageOptions
{
    public const string SectionName = "StorageSettings";

    [Required]
    public string PythonProjectImagesPath { get; set; } = string.Empty;
}
