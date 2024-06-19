using Microsoft.Extensions.Configuration;
using SemanticKernel.Config;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ConsoleChat;

public static class Setup
{
    public static void Initialize(out AzureOpenAiConfig azureOpenAiConfig, out AzureSpeechConfig azureSpeechConfig, out SpotifyConfig spotifyConfig)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("Config/AzureOpenAI.json")
            .AddJsonFile("Config/AzureSpeech.json")
            .AddJsonFile("Config/Spotify.json")
            
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();

        azureOpenAiConfig = ReadConfiguration<AzureOpenAiConfig>(config, "AzureOpenAI");
        azureSpeechConfig = ReadConfiguration<AzureSpeechConfig>(config, "AzureSpeech");
        spotifyConfig = ReadConfiguration<SpotifyConfig>(config, "Spotify");
    }
    
    private static T ReadConfiguration<T>(IConfiguration configuration, string sectionName) where T : SemanticKernel.Config.IConfiguration
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        var config = configuration.GetSection(sectionName).Get<T>();
        if (config is null)
            throw new Exception($"Invalid {typeof(T).Name}: configuration is missing!");

        return config;
    }
}
