using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Brokers;

internal interface IAgenticConversationBroker
{
    ValueTask<Conversation> CreateConversationAsync(Prompt prompt);
    ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId);
    ValueTask<Stream> PostPromptAsync(Prompt prompt);
}