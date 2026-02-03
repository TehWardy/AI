using System.Text;
using TehWardy.AI.Models;
using TehWardy.AI.Orchestrations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Coordinations;

internal class AgenticConversationCoordinationService(
    IAgenticConversationContextOrchestrationService agenticConversationContextOrchestrationService,
    IRunbookOrchestrationService runbookOrchestrationService)
        : IAgenticConversationCoordinationService
{
    public ValueTask<Conversation> CreateConversationAsync(Prompt prompt) =>
        agenticConversationContextOrchestrationService
            .CreateConversationAsync(prompt);

    public ValueTask<List<Conversation>> RetrieveAllConversationsAsync() =>
        agenticConversationContextOrchestrationService.RetrieveAllConversationsAsync();

    public async ValueTask<Conversation> RetrieveConversationByIdAsync(Guid conversationId) =>
        await agenticConversationContextOrchestrationService
            .RetrieveConversationByIdAsync(conversationId);

    public async IAsyncEnumerable<Token> InferStreamingAsync(Prompt prompt)
    {
        ConversationContext context =
            await agenticConversationContextOrchestrationService
                .RetrieveConversationContextAsync(prompt);

        if (prompt.ConversationId == Guid.Empty)
        {
            yield return new Token
            {
                Thought = "New Conversation Started",
                Content = context.Conversation.Id.ToString()
            };
        }

        if (context.Agent.UIToolName is not null)
        {
            yield return new Token
            {
                Thought = "UI Tool Recommendation",
                Content = context.Agent.UIToolName
            };
        }

        RunbookOrchestrationRequest runbookOrchestrationRequest = new()
        {
            AgentName = context.Agent.Name,
            ConversationId = prompt.ConversationId,
            Input = prompt.Input,
            History = context.Conversation.History,
            RunbookName = context.Agent.RunbookName
        };

        yield return new Token
        {
            Thought = $"Thinking ...\n"
        };

        IAsyncEnumerable<Token> response = runbookOrchestrationService
            .ExecuteRunbookOrchestrationRequestAsync(runbookOrchestrationRequest);

        await foreach (var token in response)
            yield return token;

        await agenticConversationContextOrchestrationService
            .SaveConversationContextAsync(context);
    }
}