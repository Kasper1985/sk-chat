using System.ComponentModel;
using Common.StaticFunctions;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Agents.EmailAgent;

public class Plugin
{
    [KernelFunction]
    [Description("Senda an email to a recipient")]
    public async Task SendEmail(
        Kernel kernel,
        [Description("Semicolon delimitated list of emails of the recipients")]
        string recipientEmails,
        string subject,
        string body)
    {
        if (string.IsNullOrWhiteSpace(recipientEmails))
            ConsoleMessaging.PluginMessage("[Cannot send email: recipient email is missing!]");
        if (string.IsNullOrWhiteSpace(subject))
            ConsoleMessaging.PluginMessage("[Cannot send email: subject is missing!]");
        if (string.IsNullOrWhiteSpace(body))
            ConsoleMessaging.PluginMessage("[Cannot send email: body is missing!]");
        
        ConsoleMessaging.PluginMessage($"""
            To: {recipientEmails}
            Subject: {subject}
            
            {body}                           
            """);
        ConsoleMessaging.PluginMessage("[Email sent successfully!]");
    }
        
}
