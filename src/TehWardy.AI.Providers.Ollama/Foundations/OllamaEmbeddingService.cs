using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Brokers;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal class OllamaEmbeddingService(IOllamaEmbeddingBroker ollamaEmbeddingBroker)
    : IOllamaEmbeddingService
{
    public async ValueTask<float[]> EmbedAsync(EmbeddingRequest embeddingRequest)
    {
        OllamaEmbedRequest ollamaRequest =
            CreateOllamaEmbedRequest(embeddingRequest);

        OllamaEmbedResponse ollamaResponse =
            await ollamaEmbeddingBroker.EmbedAsync(ollamaRequest);

        float[][] embeddings = (ollamaResponse.Embeddings ?? [])
            .Select(v => v.Select(x => (float)x).ToArray())
            .ToArray();

        return embeddings.Length > 0 ? embeddings[0] : [];
    }

    static OllamaEmbedRequest CreateOllamaEmbedRequest(
        EmbeddingRequest embeddingRequest) => new()
        {
            Model = embeddingRequest.ModelName,
            Inputs = [embeddingRequest.Input],
            Dimensions = 1536,
            Truncate = true
        };
}
