using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Ollama.Orchestrations;
internal interface IOllamaEmbeddingOrchestrationService
{
    ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest);
}