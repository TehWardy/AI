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

        string tokenContent;

        while (!reader.EndOfStream)
        {
            var line = (await reader.ReadLineAsync())?.TrimStart("data: ".ToArray());

            if (line is null || line == "[DONE]")
                break;

            if (line.Length == 0)
                continue;

            Token token = JsonSerializer.Deserialize<Token>(line);
            tokenContent = token.Choices[0]?.Delta?.Content;

            if(tokenContent is not null)
                yield return token.Choices[0]?.Delta?.Content;
        }
    }
}