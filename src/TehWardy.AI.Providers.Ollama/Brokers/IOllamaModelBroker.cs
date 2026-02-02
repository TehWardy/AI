using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal interface IOllamaModelBroker
{
    ValueTask<OllamaTagsResponse> ListTagsAsync();
    ValueTask<OllamaPullResponse> PullAsync(string model);
    ValueTask<OllamaModel> GetModelDetailsAsync(string modelName);
}