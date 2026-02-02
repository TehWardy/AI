using System.Diagnostics;
using System.Text.Json;
using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Agents.Runbooks.Orchestrations;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Processings;

namespace TehWardy.AI.Runbooks.Orchestrations;

internal class ToolCallingRunbookStepHandlerOrchestrationService(
    IToolExecutionProcessingService toolExecutionProcessingService,
    IRunbookStepExecutionResultProcessingService<List<ToolExecutionToken>> runbookStepExecutionResultProcessingService,
    IParameterParsingBroker parameterParsingBroker)
        : IToolCallingRunbookStepHandlerOrchestrationService
{
    static string[] toolOutputs = ["[tool:start]", "[tool:done]", "[tool:error]", "[tool:denied]"];

    public async IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest request)
    {
        // Parameters
        bool emitTokens = parameterParsingBroker
            .GetBool(request.Step.Parameters, "EmitTokens", defaultValue: false);

        int maxCalls = parameterParsingBroker
            .GetInt(request.Step.Parameters, "MaxCalls", defaultValue: 3);

        string sourceKey = parameterParsingBroker
            .GetString(request.Step.Parameters, "SourceKey", defaultValue: "LastResult");

        ToolCall[] toolCalls = GetToolCallsFromState(request, sourceKey, maxCalls);

        var results = new List<ToolExecutionToken>();

        IAsyncEnumerable<ToolCallExecutionToken> callResults =
            toolExecutionProcessingService.ExecuteCalls(request, toolCalls);

        await foreach (ToolCallExecutionToken outcome in callResults)
        {
            if (outcome.Result is not null)
            {
                Debug.WriteLine($"[tool result]\n{JsonSerializer.Serialize(outcome.Result)}");

                if (toolOutputs.Any(outcome.Content.StartsWith))
                    results.Add(outcome.Result);
            }

            if (emitTokens)
            {
                yield return new Token
                {
                    Content = outcome.Content
                };
            }
        }

        string emitCallsMadeTo = parameterParsingBroker
            .GetString(request.Step.Parameters, "EmitCallsMadeTo", defaultValue: null);

        if (emitCallsMadeTo is not null)
            request.RunbookState.Variables[emitCallsMadeTo] = results.Count > 0;

        await runbookStepExecutionResultProcessingService
            .StoreResultsInExecutionContextAsync(request, results);
    }

    static ToolCall[] GetToolCallsFromState(RunbookStepExecutionRequest req, string sourceKey, int maxCalls)
    {
        ToolCall[] toolCalls = [];

        if (!req.RunbookState.Variables.TryGetValue(sourceKey, out var value) || value is null)
            toolCalls = Array.Empty<ToolCall>();

        if (value is Token token && token.ToolCalls is not null)
            toolCalls = [.. token.ToolCalls.Take(Math.Max(0, maxCalls))];

        if (value is IEnumerable<ToolCall> enumerableCalls)
            toolCalls = [.. enumerableCalls.Take(Math.Max(0, maxCalls))];

        return toolCalls;
    }
}