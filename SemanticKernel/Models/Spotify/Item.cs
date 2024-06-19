using System.Text.Json.Serialization;

namespace SemanticKernel.Models.Spotify;

public class Item
{
    public string Id { get; init; }
    public Album Album { get; set; }
    public IEnumerable<Artist> Artists { get; init; }
    [JsonPropertyName("available_markets")]
    public IEnumerable<string> AvailableMarkets { get; init; }
    [JsonPropertyName("disc_number")]
    public int DiscNumber { get; init; }
    [JsonPropertyName("duration_ms")]
    public int DurationMs { get; init; }
    public bool Explicit { get; init; }
    [JsonPropertyName("external_ids")]
    public IDictionary<string, string> ExternalIds { get; init; }
    [JsonPropertyName("external_urls")]
    public IDictionary<string, string> ExternalUrls { get; init; }
    public string Href { get; init; }
    [JsonPropertyName("is_local")]
    public bool IsLocal { get; init; }
    public string Name { get; init; }
    public int Popularity { get; init; }
    [JsonPropertyName("preview_url")]
    public string PreviewUrl { get; init; }
    [JsonPropertyName("track_number")]
    public int TrackNumber { get; init; }
    public string Type { get; init; }
    public string Uri { get; init; }
}
