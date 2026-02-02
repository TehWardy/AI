using System.Text;
using System.Text.Json;
using TehWardy.AI.Brokers;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Fundations;

internal class AgentService(IAgentBroker agentBroker) : IAgentService
{
    public async ValueTask<Agent> InferAgentFromPromptAsync(Prompt prompt)
    {
        var agentJsonBuilder = new StringBuilder();

        try
        {
            Token agentToken = await agentBroker
                .InferAgentFromPromptAsync(prompt);

            return JsonSerializer.Deserialize<Agent>(agentToken.Content);
        }
        catch (Exception ex)
        {
            prompt.Input += $"\nThis previous response is invalid ...\n" +
                $"{agentJsonBuilder} due to the following deserialization exception:\n" +
                $"{ex.Message}";

            return await InferAgentFromPromptAsync(prompt);
        }
    }
}