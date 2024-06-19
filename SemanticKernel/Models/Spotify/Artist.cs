using System.Text.Json.Serialization;

namespace SemanticKernel.Models.Spotify;

public class Artist
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string Uri { get; init; }
    public string Href { get; init; }
    [JsonPropertyName("external_urls")]
    public IDictionary<string, string> ExternalUrls { get; init; }
}
