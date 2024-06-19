namespace SemanticKernel.Models.Spotify;

public class Tracks
{
    public string Href { get; init; }
    public IEnumerable<Item> Items { get; init; }
    public int Limit { get; init; }
    public string Next { get; init; }
    public int Offset { get; init; }
    public string Previous { get; init; }
    public int Total { get; init; }
}
