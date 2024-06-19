using System.Text.Json.Serialization;

namespace SemanticKernel.Models.Spotify;

public class Device
{
    public string Id { get; init; }
    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }
    [JsonPropertyName("is_private_session")]
    public bool IsPrivateSession { get; init; }
    [JsonPropertyName("is_restricted")]
    public bool IsRestricted { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    [JsonPropertyName("volume_percent")]
    public int VolumePercent { get; init; }
    [JsonPropertyName("supports_volume")]
    public bool SupportsVolume { get; init; }
}
