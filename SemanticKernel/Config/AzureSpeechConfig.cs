namespace SemanticKernel.Config;

public class AzureSpeechConfig : IConfiguration
{
    public string SubscriptionKey { get; set; }
    public string Region { get; set; }
    public string VoiceName { get; set; }
    public string Language { get; set; }
}
