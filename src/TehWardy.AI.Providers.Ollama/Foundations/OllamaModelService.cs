using System.Collections.Concurrent;
using TehWardy.AI.Providers.Ollama.Brokers;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal class OllamaModelService(IOllamaModelBroker modelBroker) : IOllamaModelService
{
    private readonly ConcurrentDictionary<string, Lazy<ValueTask>> pulls =
        new(StringComparer.OrdinalIgnoreCase);

    public async ValueTask DownloadModelAsync(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.", nameof(model));

        var tags = await modelBroker.ListTagsAsync();

        if (tags.Models.Any(m => string.Equals(m.Name, model, StringComparison.OrdinalIgnoreCase)))
            return;

        var lazy = pulls.GetOrAdd(
            model,
            m => new Lazy<ValueTask>(() => PullModelAsync(m)));

        try
        {
            await lazy.Value;
        }
        finally
        {
            pulls.TryRemove(model, out _);
        }
    }

    public ValueTask<OllamaModel> GetModelDetailsAsync(string modelName) =>
        modelBroker.GetModelDetailsAsync(modelName);

    private async ValueTask PullModelAsync(string model)
    {
        var result = await modelBroker.PullAsync(model);

        if (!string.Equals(result.Status, "success", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(result.Error ?? $"Failed to pull model '{model}'.");
    }
}