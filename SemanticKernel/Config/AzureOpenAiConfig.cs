namespace SemanticKernel.Config;

public class AzureOpenAiConfig : IConfiguration
{
    public string Endpoint { get; set; }
    public string Key { get; set; }
    public IDictionary<string, string> Deployments { get; set; }
}
