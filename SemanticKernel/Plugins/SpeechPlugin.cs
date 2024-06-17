using System.Text.RegularExpressions;
using Common.StaticFunctions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SemanticKernel.Config;

namespace SemanticKernel.Plugins;

public class SpeechPlugin
{
    private readonly SpeechConfig _speechConfig;
    private readonly AudioConfig _audioConfig;

    public SpeechPlugin(AzureSpeechConfig config)
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        _speechConfig = SpeechConfig.FromSubscription(config.SubscriptionKey, config.Region);
        _speechConfig.SpeechSynthesisVoiceName = config.VoiceName;
        _speechConfig.SpeechSynthesisLanguage = config.Language;
        _speechConfig.SpeechRecognitionLanguage = config.Language;

        // Creates an audio configuration that points to an audio input.
        _audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    }
    
    public async Task Speak(string text)
    {
        // use the default speaker as audio output.
        using var synthesizer = new SpeechSynthesizer(_speechConfig);
        
        // Prepare string for reading
        var textToSpeak = text;
        var match = Regex.Match(text, @"!\[.*\]\(.+\)");
        if (match.Success)
            textToSpeak = text.Replace(match.Value, "");
        
        await synthesizer.SpeakTextAsync(textToSpeak);
    }

    public async Task<string> Listen()
    {
        using var speechRecognizer = new SpeechRecognizer(_speechConfig, _audioConfig);
        var result = await speechRecognizer.RecognizeOnceAsync();
        switch (result.Reason)
        {
            case ResultReason.RecognizedSpeech:
                return result.Text;
            case ResultReason.NoMatch:
                ConsoleMessaging.PluginMessage("[NOMATCH: Speech could not be recognized.]");
                return null;
            case ResultReason.Canceled:
            {
                var cancellation = CancellationDetails.FromResult(result);
                ConsoleMessaging.PluginMessage($"[CANCELED: Reason={cancellation.Reason}]");
                if (cancellation.Reason != CancellationReason.Error)
                    return null;
                ConsoleMessaging.PluginMessage($"[CANCELED: ErrorCode={cancellation.ErrorCode}]");
                ConsoleMessaging.PluginMessage($"[CANCELED: ErrorDetails={cancellation.ErrorDetails}]");
                return null;
            }
            default:
                return null;
        }
    }
}
