
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal interface IOllamaModelService
{
    ValueTask DownloadModelAsync(string model);
    ValueTask<OllamaModel> GetModelDetailsAsync(string modelName);
}