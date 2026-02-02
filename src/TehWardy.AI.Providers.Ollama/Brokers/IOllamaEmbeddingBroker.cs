using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal interface IOllamaEmbeddingBroker
{
    ValueTask<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest ollamaEmbedRequest);
}