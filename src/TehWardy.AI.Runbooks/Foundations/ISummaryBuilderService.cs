namespace TehWardy.AI.Runbooks.Foundations;

internal interface ISummaryBuilderService<TResult>
{
    ValueTask<string> SummarizeAsync(TResult @object);
}
