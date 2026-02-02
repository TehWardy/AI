using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Foundations;

namespace TehWardy.AI.Providers.Ollama.Orchestrations;

internal class OllamaEmbeddingOrchestrationService(
    IOllamaModelService ollamaModelService,
    IOllamaEmbeddingService ollamaEmbeddingService)
        : IOllamaEmbeddingOrchestrationService
{
    public async ValueTask<float[]> EmbedAsync(
        EmbeddingRequest embeddingRequest)
    {
        try
        {
            return await ollamaEmbeddingService
                .EmbedAsync(embeddingRequest);
        }
        catch
        {
            await ollamaModelService
                .DownloadModelAsync(embeddingRequest.ModelName);

            return await ollamaEmbeddingService
                .EmbedAsync(embeddingRequest);
        }
    }
}
