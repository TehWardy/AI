using System.Text;
using System.Text.Json;
using AIServer.LlamaCpp.Brokers;
using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Foundations;

internal class LlamaCppService : ILlamaCppService
{
    private readonly InferenceSettings settings;
    private readonly ILlamaCppBroker llamaCppBroker;

    public LlamaCppService(ILlamaCppBroker llamaCppBroker)
    {
        this.settings = new();
        this.llamaCppBroker = llamaCppBroker;
    }

    public async IAsyncEnumerable<string> SendPromptAsync(LlamaCppPrompt prompt)
    {
        // Build a Llama-3 style chat prompt for /completion (so we get min_p/top_k/repeat_penalty support)
        var compiledPrompt = Llama3Template.Compile(prompt.History);

        var request = new LlamaServerCompletionRequest
        {
            Prompt = compiledPrompt,
            Stream = true,
            CachePrompt = true,
            Temperature = settings.Temperature,
            TopP = settings.TopP,
            MinP = settings.MinP,
            TopK = settings.TopK,
            RepeatPenalty = settings.RepeatPenalty,

            NPredict = settings.MaxTokens > 0
                ? settings.MaxTokens
                : 1024, // safety cap

            Stop = settings.AntiPrompts.Length > 0
                ? settings.AntiPrompts
                : ["<|eot_id|>", "<|end|>", "<|user|>", "User:"]
        };

        using StreamReader reader = await llamaCppBroker
            .SendCompletionRequestAsync(request);

        var assistantBuilder = new StringBuilder();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);

            if (line is null)
                break;

            if (line.Length == 0 || line.StartsWith(":"))
                continue; // keep-alives/comments

            if (!line.StartsWith("data:"))
                continue;

            var payload = line.AsSpan(5).Trim(); // after "data:"

            if (payload.SequenceEqual("[DONE]".AsSpan()))
                break;

            LlamaServerStreamChunk chunk = JsonSerializer
                .Deserialize<LlamaServerStreamChunk>(payload);

            if (chunk?.Content is { Length: > 0 } piece)
            {
                assistantBuilder.Append(piece);
                yield return piece;
            }

            if (chunk?.Stop == true)
                break;
        }

        var assistantText = assistantBuilder.ToString();
        var trimmedContent = TrimAtStop(assistantText, settings.AntiPrompts);
    }

    static string TrimAtStop(string text, string[] stops)
    {
        if (stops == null || stops.Length == 0)
            return text;

        var idx = -1;

        foreach (var s in stops)
        {
            var i = text.IndexOf(s, StringComparison.Ordinal);

            if (i >= 0)
                idx = idx < 0
                    ? i
                    : Math.Min(idx, i);
        }

        return idx >= 0
            ? text[..idx]
            : text;
    }
}
