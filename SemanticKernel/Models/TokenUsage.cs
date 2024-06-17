using Azure.AI.OpenAI;

namespace SemanticKernel.Models;

public class TokenUsage
{
    public int CompletionTokens { get; private set; }
    public int PromptTokens { get; private set; }
    public int TotalTokens { get; private set; }
    
    public void AddCompletionsUsage(object completionsUsage)
    {
        if (completionsUsage is not CompletionsUsage usage) return;
        CompletionTokens += usage.CompletionTokens;
        PromptTokens += usage.PromptTokens;
        TotalTokens += usage.TotalTokens;
    }
}
