using System.Text.Json;
using AIServer.Ollama.Brokers;
using AIServer.Ollama.Models;

namespace AIServer.Ollama.Foundations;

internal class OllamaConversationService : IOllamaConversationService
{
    private readonly IOllamaConversationBroker conversationBroker;

    public OllamaConversationService(IOllamaConversationBroker conversationBroker) =>
        this.conversationBroker = conversationBroker;

    public async IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt)
    {
        using Stream responseStream = await conversationBroker
            .SendPromptAsync(prompt);

        var reader = new StreamReader(responseStream);
        string line = await reader.ReadLineAsync();

        while (!reader.EndOfStream && line is not null)
        {
            ResponseToken token = JsonSerializer.Deserialize<ResponseToken>(line);
            yield return token;

            if (token.Done) break;

            line = await reader.ReadLineAsync();
        }
    }
}