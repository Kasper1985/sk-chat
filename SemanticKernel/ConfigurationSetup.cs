﻿using Microsoft.SemanticKernel;
using SemanticKernel.Config;
using SemanticKernel.Plugins;

namespace SemanticKernel;

public static class ConfigurationSetup
{
    public static Kernel ConfigureKernel(AzureOpenAiConfig config)
    {
        var kernelBuilder = Kernel.CreateBuilder();
        
        kernelBuilder.AddAzureOpenAIChatCompletion(
            config.Deployments["gpt-4o"],
            config.Endpoint,
            config.Key);

        #pragma warning disable SKEXP0010
        kernelBuilder.AddAzureOpenAITextToImage(
            config.Deployments["dall-e"],
            config.Endpoint,
            config.Key);
        #pragma warning restore SKEXP0010
        
        #pragma warning disable SKEXP0001
        kernelBuilder.AddAzureOpenAIAudioToText(
            config.Deployments["whisper"],
            config.Endpoint,
            config.Key);
        #pragma warning restore SKEXP0001

        // Add plugins
        kernelBuilder.Plugins.AddFromType<FileSystemPlugin>();
        kernelBuilder.Plugins.AddFromType<LightPlugin>();
        kernelBuilder.Plugins.AddFromType<FibonacciPlugin>();
        kernelBuilder.Plugins.AddFromType<ImagePlugin>();
        
        return kernelBuilder.Build();
    }
}
