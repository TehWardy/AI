using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Foundations;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Processings;

namespace TehWardy.AI.Agents.Runbooks.Processings;

internal class RunbookStepExecutionResultProcessingService<TResult>(
    IParameterParsingBroker parameterParser,
    ISummaryBuilderService<TResult> summaryBuilderService)
    : IRunbookStepExecutionResultProcessingService<TResult>
{
    public async ValueTask StoreResultsInExecutionContextAsync(RunbookStepExecutionRequest request, TResult result)
    {
        bool appendToHistory = parameterParser
            .GetBool(request.Step.Parameters, "AppendToHistory", defaultValue: false);

        string storeAs = parameterParser
            .GetString(request.Step.Parameters, "StoreAs", defaultValue: $"{request.Step.Name}.Result");

        request.RunbookState.Variables[storeAs] = result;
        request.RunbookState.Variables["LastResult"] = result;

        if (appendToHistory is true)
        {
            string summary = await summaryBuilderService
                .SummarizeAsync(result);

            summary = TrimSumamry(summary, 500);

            if (request.Step.StepType == "toolcall")
            {
                request.RunbookExecutionRequest.ConversationHistory.Add(new ChatMessage
                {
                    Role = "tool",
                    Message = summary
                });
            }
            else
            {
                request.RunbookExecutionRequest.ConversationHistory.Add(new ChatMessage
                {
                    Role = "assistant",
                    Message = summary
                });
            }
        }
    }

    static string TrimSumamry(string summary, int maxChars)
    {
        string text = summary ?? string.Empty;

        if (string.IsNullOrWhiteSpace(text))
            return "(no output)";

        text = text.Trim();

        return text.Length <= maxChars
            ? text
            : text.Substring(0, maxChars) + "…";
    }
}