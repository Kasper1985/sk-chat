using Common.StaticFunctions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Models;
using SemanticKernel.Plugins;

namespace ConsoleChat;

public class Chat(Kernel kernel, SpeechPlugin speechPlugin = null)
{
    public async Task Start()
    {
        var chatHistory = new ChatHistory();
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var usage = new TokenUsage();
        
        // Start conversation
        var retry = true;
        while (retry)
        {
            // Read user input from microphone or console
            string userInput;
            if (speechPlugin is not null)
            {
                ConsoleMessaging.SystemMessage("[Listening...]");
                userInput = await speechPlugin.Listen();
                if (userInput is null)
                {
                    ConsoleMessaging.SystemMessage("[Try again? (y/n)]");
                    var tryAgain = Console.ReadLine();
                    if (tryAgain?.ToLower() != "y")
                    {
                        retry = false;
                        continue;
                    }
                }
                else
                    ConsoleMessaging.UserMessage(userInput);
            }
            else
            {
                userInput = Console.ReadLine();
            }
            
            // Skip empty inputs
            if (string.IsNullOrWhiteSpace(userInput)) continue;
            
            // Add user input to history
            chatHistory.AddUserMessage(userInput!);
            
            // Enable auto function calling
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            
            // Ask AI and get the result of the conversation
            var result = await chatService.GetChatMessageContentsAsync(chatHistory, executionSettings, kernel);
            var response = "";           
            foreach (var messageContent in result)
            {
                response += messageContent.Content;
                usage.AddCompletionsUsage(messageContent.Metadata!["Usage"]);
            }
            
            // Speak and/or write the response to console
            if (speechPlugin is not null)
            {
                var speakTask = speechPlugin.Speak(response);
                var writeTask = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    ConsoleMessaging.AssistantMessageFluent(response);
                });
                await Task.WhenAll(speakTask, writeTask);
            }
            else
            {
                ConsoleMessaging.AssistantMessageFluent(response, 100);
            }
            
            ConsoleMessaging.SystemMessage($"[Used tokens: {usage.CompletionTokens} completion tokens, {usage.PromptTokens} prompt tokens, {usage.TotalTokens} total tokens]");
            
            // Add the message from the agent to the chat history
            chatHistory.AddMessage(AuthorRole.Assistant, response);
            Console.ResetColor();
        }
        
        ConsoleMessaging.SystemMessage("[End of conversation]");
    }
}
