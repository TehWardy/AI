using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Runbooks.Foundations;

internal class AccumulatedTokenSummaryBuilderService
    : ISummaryBuilderService<AccumulatedToken>
{
    public ValueTask<string> SummarizeAsync(AccumulatedToken @object)
    {
        string text = @object.Content ?? string.Empty;

        if (string.IsNullOrWhiteSpace(text))
            return ValueTask.FromResult("(no output)");

        return ValueTask.FromResult(text.Trim());
    }
}