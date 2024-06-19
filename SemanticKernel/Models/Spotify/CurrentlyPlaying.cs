namespace SemanticKernel.Models.Spotify;

public class CurrentlyPlaying
{
    public string CurrentlyPlayingType { get; init; }
    public Context Context { get; init; }
    public int ProgressMs { get; init; }
    public Item Item { get; init; }
    public object Actions { get; init; }
    public bool is_playing { get; init; }
    public long Timestamp { get; init; }
}
