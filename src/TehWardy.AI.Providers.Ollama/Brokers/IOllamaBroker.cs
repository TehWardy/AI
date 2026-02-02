using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal interface IOllamaBroker
{
    ValueTask<Stream> SendPromptForStreamingAsync(OllamaPrompt prompt);
    ValueTask<OllamaToken> SendPromptAsync(OllamaPrompt prompt);
}