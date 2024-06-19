using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.StaticFunctions;
using Microsoft.SemanticKernel;
using SemanticKernel.Config;
using SemanticKernel.Models.Spotify;

namespace SemanticKernel.Plugins;

public class SpotifyPlugin
{
    private readonly SpotifyConfig _spotifyConfig;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private const string PlayerEndpoint = "me/player";
    private static SpotifyToken _token = new();
    
    
    public SpotifyPlugin(SpotifyConfig spotifyConfig)
    {
        _spotifyConfig = spotifyConfig;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.BaseAddress = new Uri(_spotifyConfig.ApiBaseAddress);
    }


    [KernelFunction("AvailableDevices")]
    [Description("Get available devices to play music.")]
    public async Task<IEnumerable<Device>> GetAvailableDevices()
    {
        if (!SpotifyToken.IsValid(_token))
            await GetToken();
        
        AddAuthHeader(_httpClient, _token);
        
        var result = await _httpClient.GetAsync(new Uri($"{PlayerEndpoint}/devices", UriKind.Relative));
        var devices = await Deserialize<DevicesResult>(result);
        if (devices is not null)
            return devices.Devices;
        
        ConsoleMessaging.PluginMessage("[Failed to get available devices]");
        ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
        return null;

    }
    
    [KernelFunction("PlayMusic")]
    [Description("Start playing music from music streaming service.")]
    public async Task<string> PlayMusic(Device device, string lyric = null, string genre = null)
    {
        if (!SpotifyToken.IsValid(_token))
            await GetToken();
        
        AddAuthHeader(_httpClient, _token);

        HttpResponseMessage result;
        if (!string.IsNullOrWhiteSpace(lyric))
        {
            result = await _httpClient.GetAsync(new Uri($"search?q={lyric}&type=track", UriKind.Relative));
            var searchResults = await Deserialize<SearchResults>(result);
            if (searchResults is null)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
                return "Failed to request music by lyric";
            }
            
            var tracks = searchResults.Tracks;
            if (!tracks.Items?.Any() ?? true)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage("[No music found]");
                return $"The {lyric} was not found in the music library.";
            }
            var item = tracks.Items.First();
            var content = new StringContent(JsonSerializer.Serialize(new { uris = new[] { item.Uri } }, SerializerOptions), Encoding.UTF8, "application/json");
            result = await _httpClient.PutAsync(new Uri($"{PlayerEndpoint}/play?device_id={device.Id}", UriKind.Relative), content);
            if (!result.IsSuccessStatusCode)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
                return $"Failed to play music, because an error occurred: {result.ReasonPhrase}";
            }
            
            ConsoleMessaging.PluginMessage($"[Start playing music: {item.Name} by {string.Join(", ", item.Artists.Select(a => a.Name))}]");
            return $"{item.Name} by {string.Join(", ", item.Artists.Select(a => a.Name))}";
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            result = await _httpClient.GetAsync(new Uri($"search?q=genre:%22{genre}%22&type=track", UriKind.Relative));
            var searchResults = await Deserialize<SearchResults>(result);
            if (searchResults is null)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
                return "Failed to request music by genre";
            }

            var tracks = searchResults.Tracks;
            if (!tracks.Items?.Any() ?? true)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage("[No music found]");
                return $"No music found for genre: {genre}";
            }
            
            var item = tracks.Items.First();
            var content = new StringContent(JsonSerializer.Serialize(new { uris = new[] { item.Uri } }, SerializerOptions), Encoding.UTF8, "application/json");
            result = await _httpClient.PutAsync(new Uri($"{PlayerEndpoint}/play?device_id={device.Id}", UriKind.Relative), content);
            if (!result.IsSuccessStatusCode)
            {
                ConsoleMessaging.PluginMessage("[Failed to play music]");
                ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
                return $"Failed to play music. An error occurred: {result.ReasonPhrase}.";
            }
            
            ConsoleMessaging.PluginMessage($"[Start playing music: {item.Name} by {string.Join(", ", item.Artists.Select(a => a.Name))}]");
            return $"{item.Name} by {string.Join(", ", item.Artists.Select(a => a.Name))}";
        }
        
        result = await _httpClient.PutAsync(new Uri($"{PlayerEndpoint}/play?device_id={device.Id}", UriKind.Relative), null);
        if (!result.IsSuccessStatusCode)
        {
            ConsoleMessaging.PluginMessage("[Failed to play music]");
            ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
            return $"Could not play music because: {result.ReasonPhrase}";
        }

        var currentlyPlaying = await NowPlaying();
        ConsoleMessaging.PluginMessage($"[Start playing music...]");
        return currentlyPlaying;
    }

    [KernelFunction("StopMusic")]
    [Description("Stop playing music from music streaming service.")]
    public async Task StopMusic()
    {
        await PauseMusic();
        ConsoleMessaging.PluginMessage("[Stop playing music...]");
    }
    
    [KernelFunction("PauseMusic")]
    [Description("Pause playing music from music streaming service.")]
    public async Task PauseMusic()
    {
        if (!SpotifyToken.IsValid(_token))
            await GetToken();
        
        AddAuthHeader(_httpClient, _token);
        var result = await _httpClient.PutAsync(new Uri($"{PlayerEndpoint}/pause", UriKind.Relative), null);
        if (!result.IsSuccessStatusCode)
            ConsoleMessaging.PluginMessage("[Failed to pause music]");
        ConsoleMessaging.PluginMessage("[Paused playing music]");
    }
    
    [KernelFunction("ResumeMusic")]
    [Description("Resume playing music from music streaming service.")]
    public async Task ResumeMusic()
    {
        if (!SpotifyToken.IsValid(_token))
            await GetToken();
        
        AddAuthHeader(_httpClient, _token);
        
        var result = await _httpClient.GetAsync(new Uri($"{PlayerEndpoint}/currently-playing", UriKind.Relative));
        var currentlyPlaying = await Deserialize<CurrentlyPlaying>(result);
        if (currentlyPlaying is null)
        {
            ConsoleMessaging.PluginMessage("[Failed to resume playing music]");
            return;
        }
        
        if (currentlyPlaying.is_playing)
            ConsoleMessaging.PluginMessage("[Music is already playing]");
        
        result = await _httpClient.PutAsync(new Uri($"{PlayerEndpoint}/play", UriKind.Relative), null);
        if (!result.IsSuccessStatusCode)
            ConsoleMessaging.PluginMessage("[Failed to resume playing music]");
        ConsoleMessaging.PluginMessage("[Resume playing music...]");
    }
    
    [KernelFunction("NowPlaying")]
    [Description("Get the current playing music from music streaming service.")]
    public async Task<string> NowPlaying()
    {
        if (!SpotifyToken.IsValid(_token))
            await GetToken();
        
        AddAuthHeader(_httpClient, _token);
        var result = await _httpClient.GetAsync(new Uri($"{PlayerEndpoint}/currently-playing", UriKind.Relative));
        var currentlyPlaying = await Deserialize<CurrentlyPlaying>(result);
        if (currentlyPlaying is null)
        {
            ConsoleMessaging.PluginMessage("[Failed to get the current playing music]");
            ConsoleMessaging.PluginMessage($"[Error: {result.ReasonPhrase}]");
            return null;
        }
        
        ConsoleMessaging.PluginMessage("[Got the current playing music...]");
        return currentlyPlaying.is_playing ? $"{currentlyPlaying.Item.Name} by {string.Join(", ", currentlyPlaying.Item.Artists.Select(a => a.Name))}" : "No music is playing";
    }
    
    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
    
    private static void AddAuthHeader(HttpClient httpClient, SpotifyToken token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
    }
    
    private async Task GetToken()
    {
        // Initialize HTTP client to get token
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Prepare token request message
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _spotifyConfig.TokenEndpoint);
        requestMessage.Headers.Add("Authorization", $"Basic {Base64Encode($"{_spotifyConfig.ClientId}:{_spotifyConfig.ClientSecret}")}");
        requestMessage.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
        
        var result = await httpClient.SendAsync(requestMessage);
        _token = await Deserialize<SpotifyToken>(result);
        
        if (_token is null)
            ConsoleMessaging.PluginMessage("[Failed to get token]");
    }
    
    private static async Task<T> Deserialize<T>(HttpResponseMessage response) where T : class
    {
        ArgumentNullException.ThrowIfNull(response);

        if (!response.IsSuccessStatusCode)
            return null;
        
        await using var stream = await response.Content.ReadAsStreamAsync();
        if (stream.Length == 0)
            return default;
        return await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions);
    }
}
