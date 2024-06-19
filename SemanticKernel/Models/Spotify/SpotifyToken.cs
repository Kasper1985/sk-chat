namespace SemanticKernel.Models.Spotify;

public class SpotifyToken
{
    public string AccessToken { get; init; }
    public string TokenType { get; init; }
    public int ExpiresIn { get; init; }
    
    public static bool IsValid(SpotifyToken token)
    {
        return token is not null && !string.IsNullOrWhiteSpace(token.AccessToken);
    }
};
