using Microsoft.Extensions.Configuration;
using SemanticKernel.Config;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ConsoleChat;

public static class Setup
{
    public static void Initialize(out AzureOpenAiConfig azureOpenAiConfig, out AzureSpeechConfig azureSpeechConfig)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("Config/AzureOpenAI.json")
            .AddJsonFile("Config/AzureSpeech.json")
            
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();

        azureOpenAiConfig = ReadConfiguration<AzureOpenAiConfig>(config, "AzureOpenAI");
        azureSpeechConfig = ReadConfiguration<AzureSpeechConfig>(config, "AzureSpeech");
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
