using TehWardy.AI.Models;
using TehWardy.AI.Providers.AI;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Brokers;

internal class AgentBroker(IConversationRouter conversationRouter, AgentConfiguration agentConfiguration)
    : IAgentBroker
{
    public ValueTask<Token> InferAgentFromPromptAsync(Prompt prompt)
    {
        InferrenceRequest inferrenceRequest = new()
        {
            LLMProviderName = agentConfiguration.RoutingProviderName,
            LLMModelName = agentConfiguration.RoutingModelName,
            ContextLength = agentConfiguration.RoutingContextLength,
            Context =
            [
                new ChatMessage { Role = "user", Message = prompt.Input }
            ]
        };

        return conversationRouter.InferRouteAsync<Agent>(inferrenceRequest);
    }
}