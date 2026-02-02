using TehWardy.AI.Brokers;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Fundations;

internal class ConversationService(IConversationBroker conversationBroker)
    : IConversationService
{
    public async ValueTask<Conversation> CreateConversationAsync(Prompt prompt)
    {
        Conversation conversation = new()
        {
            Id = Guid.NewGuid(),
            History =
            [
                new ChatMessage
                {
                    Role = "user",
                    Message = prompt.Input
                }
            ]
        };

        await conversationBroker.SaveConversationAsync(conversation);

        return conversation;
    }

    public ValueTask<List<Conversation>> RetrieveAllConversationsAsync() =>
        conversationBroker.RetrieveAllConversationsAsync();

    public ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId) =>
        conversationBroker.GetConversationByIdAsync(conversationId);

    public async ValueTask<Conversation> RetrieveConversationByIdAsync(Guid conversationId)
    {
        Conversation conversation = null;

        if (conversationId != Guid.Empty)
            conversation = await conversationBroker
                .GetConversationByIdAsync(conversationId);

        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = conversationId,
                History = []
            };

            await conversationBroker.SaveConversationAsync(conversation);
        }

        return conversation;
    }
}