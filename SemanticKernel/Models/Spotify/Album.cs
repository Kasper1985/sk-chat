using System.Text.Json.Serialization;

namespace SemanticKernel.Models.Spotify;

public class Album
{
    public string Id { get; init; }
    [JsonPropertyName("album_type")]
    public string AlbumType { get; init; }
    public IEnumerable<Artist> Artists { get; init; }
    [JsonPropertyName("available_markets")]
    public IEnumerable<string> AvailableMarkets { get; init; }
    [JsonPropertyName("external_urls")]
    public IDictionary<string, string> ExternalUrls { get; init; }
    public string Href { get; init; }
    public IEnumerable<Image> Images { get; init; }
    public string Name { get; init; }
    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; init; }
    [JsonPropertyName("release_date_precision")]
    public string ReleaseDatePrecision { get; init; }
    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; init; }
    public string Type { get; init; }
    public string Uri { get; init; }
}
