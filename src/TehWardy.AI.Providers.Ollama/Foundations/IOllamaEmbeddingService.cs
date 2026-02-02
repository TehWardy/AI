using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal interface IOllamaEmbeddingService
{
    ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest);
}