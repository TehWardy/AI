using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Orchestrations;
using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Ollama;

internal class OllamaEmbeddingProvider(IOllamaEmbeddingOrchestrationService ollamaEmbeddingOrchestrationService)
    : IEmbeddingProvider
{
    public ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest) =>
        ollamaEmbeddingOrchestrationService.EmbedAsync(embeddingRequest);
}