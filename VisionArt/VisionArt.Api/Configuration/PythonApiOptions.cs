namespace VisionArt.Api.Configuration;

public class PythonApiOptions
{
    public const string SectionName = "PythonApi";

    public string BaseUrl { get; set; } = "http://localhost:8000";
}
