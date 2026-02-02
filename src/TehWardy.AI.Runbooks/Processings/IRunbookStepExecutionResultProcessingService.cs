using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal interface IRunbookStepExecutionResultProcessingService<TResult>
{
    ValueTask StoreResultsInExecutionContextAsync(RunbookStepExecutionRequest request, TResult result);
}