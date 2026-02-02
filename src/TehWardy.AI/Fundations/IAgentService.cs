using TehWardy.AI.Models;

namespace TehWardy.AI.Fundations;
internal interface IAgentService
{
    ValueTask<Agent> InferAgentFromPromptAsync(Prompt prompt);
}