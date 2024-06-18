using Common.StaticFunctions;
using EmailAgent;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Agents = SemanticKernel.Agents;

// Read configuration from config files
Setup.Initialize(out var azureOpenAiConfig);

// Create the kernel
var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(
    azureOpenAiConfig.Deployments["gpt-4o"],
    azureOpenAiConfig.Endpoint,
    azureOpenAiConfig.Key);
builder.Plugins.AddFromType<Agents.EmailAgent.Plugin>();
builder.Plugins.AddFromType<Agents.EmailAgent.Planner>();
var kernel = builder.Build();

// Retrieve the chat completion service from the kernel
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Create the chat history
var chatMessages = new ChatHistory(Agents.EmailAgent.Persona.InitialPrompt);

// Start the conversation
while (true)
{
    // Get user input
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("You: ");
    chatMessages.AddUserMessage(Console.ReadLine()!);
    
    // Get the chat completions
    var executionSettings = new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    var result = await chatCompletionService.GetChatMessageContentsAsync(chatMessages, executionSettings, kernel);
    
    // Write the results to the console
    var fullMessage = string.Join("", result.Select(c => c.Content));
    ConsoleMessaging.AssistantMessageFluent(fullMessage, 50);
    
    // Add message from the agent to the chat history
    chatMessages.AddAssistantMessage(fullMessage);
}
