using System.Diagnostics;
using System.Reflection;
using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal class ToolExecutionProcessingService(IToolExecutionService toolExecutionService)
    : IToolExecutionProcessingService

{
    public async IAsyncEnumerable<ToolCallExecutionToken> ExecuteCalls(
        RunbookStepExecutionRequest request,
        ToolCall[] toolCalls)
    {
        var results = new List<ToolCallExecutionToken>();

        foreach (var call in toolCalls)
        {
            if (!IsToolAllowed(request, call))
                yield return BuildDenyToolExecutionResult(call);
            else
            {
                IAsyncEnumerable<ToolCallExecutionToken> result = ExecuteToolCallAsync(call, results);

                await foreach (var token in result)
                {
                    results.Add(token);
                    yield return token;
                }
            }
        }
    }

    async IAsyncEnumerable<ToolCallExecutionToken> ExecuteToolCallAsync(ToolCall call, List<ToolCallExecutionToken> previousResults)
    {
        Debug.WriteLine($"[tool:start] {call.ToolName}.{call.FunctionName}");

        yield return new ToolCallExecutionToken
        {
            Content = $"[tool:start] {call.ToolName}.{call.FunctionName}"
        };

        ToolExecutionToken result;

        try
        {
            object toolResult = await toolExecutionService.ExecuteToolFunctionAsync(
                call.ToolName,
                call.FunctionName,
                call.Arguments ?? new Dictionary<string, object>());

            result = CreateToolExecutionResult(call, true, output: toolResult);
        }
        catch (TargetInvocationException tie)
        {
            var real = tie.InnerException ?? tie;
            result = CreateToolExecutionResult(call, false, error: real.Message);
        }
        catch (Exception ex)
        {
            result = CreateToolExecutionResult(call, false, error: ex.Message);
        }

        if (result.Output is IAsyncEnumerable<ProcessToken> processStream)
        {
            const int tailLines = 40;
            var output = new Queue<string>(tailLines);
            var error = new Queue<string>(tailLines);

            await foreach (var processToken in processStream)
            {
                var channel = processToken.StreamSource == ProcessStreamSource.StdOut
                    ? "output"
                    : "error";

                if (!string.IsNullOrEmpty(processToken.Value))
                {
                    if (processToken.StreamSource == ProcessStreamSource.StdOut)
                        EnqueueTail(output, processToken.Value, tailLines);
                    else
                        EnqueueTail(error, processToken.Value, tailLines);

                    Debug.WriteLine($"[tool:{channel}] {processToken.Value}");

                    yield return new ToolCallExecutionToken
                    {
                        Content = $"[tool:{channel}] {processToken.Value}"
                    };
                }
            }

            result.Output = string.Join("\n", output.ToArray());
            result.Error = string.Join("\n", error.ToArray());
        }

        Debug.WriteLine(result.Succeeded
            ? $"[tool:done] {call.ToolName}.{call.FunctionName}"
            : $"[tool:error] {call.ToolName}.{call.FunctionName}");

        yield return new ToolCallExecutionToken
        {
            Content = result.Succeeded
                ? $"[tool:done] {call.ToolName}.{call.FunctionName}"
                : $"[tool:error] {call.ToolName}.{call.FunctionName}",

            Result = result
        };
    }

    static ToolCallExecutionToken BuildDenyToolExecutionResult(ToolCall call)
    {
        Debug.WriteLine($"[tool:denied] '{call.ToolName}.{call.FunctionName}' not allowed by runbook policy.");

        return new()
        {
            Content = $"[tool:denied] '{call.ToolName}.{call.FunctionName}' not allowed by runbook policy.",
            Result = new ToolExecutionToken
            {
                ToolName = call.ToolName,
                FunctionName = call.FunctionName,
                Arguments = call.Arguments,
                Succeeded = false,
                Output = null,
                Error = $"[tool:denied] '{call.ToolName}.{call.FunctionName}' not allowed by runbook policy."
            }
        };
    }

    static bool IsToolAllowed(RunbookStepExecutionRequest req, ToolCall toolCall)
    {
        ToolFunction[] allowed = req.RunbookExecutionRequest
            .Runbook.Policy?.AllowedTools?
            .Where(tool => string.Equals(tool.Name, toolCall.ToolName, StringComparison.OrdinalIgnoreCase))
            .SelectMany(tool => tool.ToolFunctions
                .Where(function => string.Equals(function.Name, toolCall.FunctionName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return allowed?.Length > 0;
    }
    static ToolExecutionToken CreateToolExecutionResult(
        ToolCall call, bool succeeded, object output = null, string error = null)
    {
        return new ToolExecutionToken
        {
            ToolName = call.ToolName,
            FunctionName = call.FunctionName,
            Arguments = call.Arguments,
            Succeeded = succeeded,
            Output = output,
            Error = error
        };
    }

    private static void EnqueueTail(Queue<string> queue, string line, int max)
    {
        if (queue.Count == max)
            queue.Dequeue();

        queue.Enqueue(line);
    }
}