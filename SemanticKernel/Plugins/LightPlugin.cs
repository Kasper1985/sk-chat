using System.ComponentModel;
using Common.StaticFunctions;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins;

public class LightPlugin
{
    public bool IsOn { get; set; } = false;
    
    [KernelFunction]
    [Description("Gets the current state of the light.")]
    public string GetState() => IsOn ? "on" : "off";
    
    [KernelFunction]
    [Description("Change the state of the light.")]
    public string ChangeState(bool newState)
    {
        IsOn = newState;
        var state = GetState();
        
        ConsoleMessaging.PluginMessage($"[Light is now {state}]");

        return state;
    }
}
