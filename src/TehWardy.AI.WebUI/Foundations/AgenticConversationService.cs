using System.Text.Json;
using TehWardy.AI.WebUI.Brokers;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Foundations;

internal class AgenticConversationService(IAgenticConversationBroker agenticConversationBroker)
    : IAgenticConversationService
{
    public ValueTask<Conversation> CreateConversationAsync(Prompt prompt) =>
        agenticConversationBroker.CreateConversationAsync(prompt);

    public ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId) =>
        agenticConversationBroker.RetrieveConversationAsync(conversationId);

    public async IAsyncEnumerable<Token> SendPromptAsync(Prompt prompt)
    {
        using Stream responseStream = await agenticConversationBroker
            .PostPromptAsync(prompt);

        var reader = new StreamReader(responseStream);

        while (await reader.ReadLineAsync() is string line)
        {
            Token token = JsonSerializer
                .Deserialize<Token>(line);

            yield return token;
        }
    }
}