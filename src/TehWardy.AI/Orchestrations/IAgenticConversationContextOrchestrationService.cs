using TehWardy.AI.Models;

namespace TehWardy.AI.Orchestrations;

internal interface IAgenticConversationContextOrchestrationService
{
    ValueTask<Conversation> CreateConversationAsync(Prompt prompt);
    ValueTask<List<Conversation>> RetrieveAllConversationsAsync();
    ValueTask<Conversation> RetrieveConversationByIdAsync(Guid converstionId);
    ValueTask<ConversationContext> RetrieveConversationContextAsync(Prompt prompt);
}