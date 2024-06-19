namespace SemanticKernel.Config;

public class SpotifyConfig : IConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthorizeEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string RedirectUri { get; set; }
    
    public string ApiBaseAddress { get; set; }
}
