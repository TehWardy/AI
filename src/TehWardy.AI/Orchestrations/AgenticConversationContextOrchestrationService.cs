using TehWardy.AI.Fundations;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Orchestrations;

internal class AgenticConversationContextOrchestrationService(
    IAgentService agentService,
    IConversationService conversationService)
        : IAgenticConversationContextOrchestrationService
{
    public ValueTask<Conversation> CreateConversationAsync(Prompt prompt) =>
        conversationService.CreateConversationAsync(prompt);

    public ValueTask<List<Conversation>> RetrieveAllConversationsAsync() =>
        conversationService.RetrieveAllConversationsAsync();

    public ValueTask<Conversation> RetrieveConversationByIdAsync(Guid converstionId) =>
        conversationService.RetrieveConversationByIdAsync(converstionId);

    public async ValueTask<ConversationContext> RetrieveConversationContextAsync(Prompt prompt)
    {
        Agent agent = await agentService.InferAgentFromPromptAsync(prompt);

        Conversation conversation = await conversationService
            .RetrieveConversationByIdAsync(prompt.ConversationId);

        ChatMessage lastMessage = conversation.History.Count > 0
            ? conversation.History[^1]
            : null;

        if (lastMessage?.Role != "user" || lastMessage.Message != prompt.Input)
        {
            conversation.History.Add(new ChatMessage
            {
                Role = "user",
                Message = prompt.Input
            });
        }

        return new ConversationContext
        {
            Agent = agent,
            Conversation = conversation
        };
    }
}
