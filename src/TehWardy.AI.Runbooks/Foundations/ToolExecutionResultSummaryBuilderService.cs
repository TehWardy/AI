using System.Text.Json;
using TehWardy.AI.Runbooks.Foundations;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Foundations;

internal class ToolExecutionResultSummaryBuilderService
    : ISummaryBuilderService<List<ToolExecutionToken>>
{
    public ValueTask<string> SummarizeAsync(List<ToolExecutionToken> @object)
    {
        List<string> summaries = [];

        foreach (ToolExecutionToken result in @object)
        {
            string status = result.Succeeded ? "SUCCESS" : "FAILED";

            string detail = result.Succeeded
                ? Summarize(result, maxChars: 500)
                : result.Error;

            summaries.Add($"- [result:{status}] {result.ToolName}.{result.FunctionName} => \n{detail}");
        }

        return ValueTask.FromResult(string.Join(Environment.NewLine, summaries));
    }

    static string Summarize(ToolExecutionToken result, int maxChars)
    {
        if (result is null)
            return "(no output)";

        if (result.Output is string s)
            return TrimToCharCount(string.IsNullOrWhiteSpace(s) ? "(no output)" : s.Trim(), maxChars);

        return TrimToCharCount(JsonSerializer.Serialize(result.Output), maxChars);
    }

    static string TrimToCharCount(string text, int maxChars)
    {
        return text.Length <= maxChars
            ? text
            : text.Substring(0, maxChars) + "…";
    }
}