using System.ComponentModel;
using Common.StaticFunctions;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins;

public class FileSystemPlugin
{
    private const string basePath = "../../../Output/";
    
    [KernelFunction("WriteFile")]
    [Description("Writes a file to the file system")]
    public string WriteFile(string path, string content)
    {
        path = Path.GetFileName(path);
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);
        
        var filePath = Path.Combine(basePath, path);
        File.WriteAllText(filePath, content);
        ConsoleMessaging.PluginMessage($"[File written to {filePath}]");
        return filePath;
    }
    
    [KernelFunction("FindFiles")]
    [Description("Finds files in the file system")]
    public string[] FindFiles(string searchPattern)
    {
        var files = Directory.GetFiles(basePath, searchPattern);
        ConsoleMessaging.PluginMessage($"[Found {files.Length} files in {basePath}]");
        return files;
    }
    
    [KernelFunction("ReadFile")]
    [Description("Reads a file from the file system")]
    public string ReadFile(string path)
    {
        path = Path.GetFileName(path);
        if (!File.Exists(Path.Combine(basePath, path)))
            return null;
        
        var filePath = Path.Combine(basePath, path);
        ConsoleMessaging.PluginMessage($"[Reading file from {filePath}]");
        return File.ReadAllText(filePath);
    }
    
    [KernelFunction("DeleteFile")]
    [Description("Deletes a file from the file system")]
    public string DeleteFile(string path)
    {
        path = Path.GetFileName(path);
        if (!File.Exists(Path.Combine(basePath, path)))
            return "File not found";
        
        var filePath = Path.Combine(basePath, path);
        File.Delete(filePath);
        ConsoleMessaging.PluginMessage($"[File deleted from {filePath}]");
        return filePath;
    }
}
