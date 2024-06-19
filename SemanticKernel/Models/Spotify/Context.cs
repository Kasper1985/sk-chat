namespace SemanticKernel.Models.Spotify;

public class Context
{
    public IDictionary<string, string> ExternalUrls { get; init; }
    public string Href { get; init; }
    public string Type { get; init; }
    public string Uri { get; init; }
}
