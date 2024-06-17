using System.ComponentModel;
using Common.StaticFunctions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace SemanticKernel.Plugins;

public class ImagePlugin
{
    [KernelFunction("CreateImage")]
    [Description("Creates am image from a text")]
    public string CreateImage(Kernel kernel, string text)
    {
        #pragma warning disable SKEXP0001
        var imageService = kernel.GetRequiredService<ITextToImageService>();

        var result = imageService.GenerateImageAsync(text, 1024, 1024, kernel).Result;
        ConsoleMessaging.PluginMessage("[Image was created]");
        return result;
        #pragma warning restore SKEXP0001
    }
}
