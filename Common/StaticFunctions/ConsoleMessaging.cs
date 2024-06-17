namespace Common.StaticFunctions;

public class ConsoleMessaging
{
    public static void PluginMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
    }
    
    public static void AssistantMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
    }
    
    public static void AssistantMessageFluent(string message, int pause = 200)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (var word in message.Split(' '))
        {
            Console.Write(word + " ");
            Thread.Sleep(pause);
        }
        Console.WriteLine();
    }
    
    public static void SystemMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
    }
    
    public static void UserMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }
}
