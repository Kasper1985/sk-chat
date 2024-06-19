using ConsoleChat;
using SemanticKernel;
using SemanticKernel.Plugins;

// Read configuration from config files
Setup.Initialize(out var azureOpenAiConfig, out var azureSpeechConfig, out var spotifyConfig);

// Configure semantic kernel
var kernel = ConfigurationSetup.ConfigureKernel(azureOpenAiConfig, spotifyConfig);

// Start chatbot
var chat = new Chat(kernel, args.Contains("-a") ? new SpeechPlugin(azureSpeechConfig) : null);
await chat.Start();
