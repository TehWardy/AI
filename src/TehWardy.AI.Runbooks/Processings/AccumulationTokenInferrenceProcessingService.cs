using System.Text;
using TehWardy.AI.Agents.Runbooks.Foundations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal class AccumulationTokenInferrenceProcessingService(
    IAccumulationTokenInferrenceService accumulationTokenInferrenceService)
        : IAccumulationTokenInferrenceProcessingService
{
    public async IAsyncEnumerable<Token> SendInferrenceRequestAsync(
        InferrenceRequest inferrenceRequest)
    {
        StringBuilder contentBuilder = new();
        StringBuilder thoughtBuilder = new();
        List<ToolCall> toolCalls = [];

        IAsyncEnumerable<Token> responseTokens = accumulationTokenInferrenceService
                .SendInferrenceRequestAsync(inferrenceRequest);

        await foreach (var token in responseTokens)
        {
            contentBuilder.Append(token.Content ?? string.Empty);
            thoughtBuilder.Append(token.Thought ?? string.Empty);
            toolCalls.AddRange(token.ToolCalls ?? []);

            yield return token;
        }

        yield return new AccumulatedToken
        {
            Content = contentBuilder.ToString(),
            Thought = thoughtBuilder.ToString(),
            ToolCalls = toolCalls.ToArray()
        };
    }
}
