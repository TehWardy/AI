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

    public async IAsyncEnumerable<string> SendPromptAsync(List<Message> conversationHistory)
    {
        var request = new LlamaServerCompletionRequest
        {
            Messages = conversationHistory,
            Stream = true,
            CachePrompt = true,
            Temperature = settings.Temperature,
            TopP = settings.TopP,
            MinP = settings.MinP,
            TopK = settings.TopK,
            RepeatPenalty = settings.RepeatPenalty,
            NPredict = settings.MaxTokens,
            Stop = settings.AntiPrompts
        };

        using var response = await llamaCppBroker.SendCompletionRequestAsync(request); 
        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (line is null || line == "data: [DONE]")
                break;

            if (line.Length == 0 || line.StartsWith(":"))
                continue; // keep-alives/comments

            if (!line.StartsWith("data:"))
                continue;

            var payload = line.AsSpan(5).Trim(); // after "data:"
            Token token = JsonSerializer.Deserialize<Token>(payload);

            if (token.Choices[0].Delta?.Content?.Length > 0)
                yield return token.Choices[0].Delta.Content;
        }
    }
}
