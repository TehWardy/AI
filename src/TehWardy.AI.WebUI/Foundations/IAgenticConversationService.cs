using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Foundations;

public interface IAgenticConversationService
{
    ValueTask<Conversation> CreateConversationAsync(Prompt prompt);
    ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId);
    IAsyncEnumerable<Token> SendPromptAsync(Prompt prompt);
}