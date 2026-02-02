using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Brokers;

internal interface IAgentBroker
{
    ValueTask<Token> InferAgentFromPromptAsync(Prompt prompt);
}