using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Processings;

namespace TehWardy.AI.Runbooks.RunbookStepHandlers;

internal class ExecuteRunbookStepHandler(IExecuteRunbookStepProcessingService executeRunbookStepProcessingService) 
    : IRunbookStepHandler
{
    public IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest runbookStepExecutionRequest) =>
        executeRunbookStepProcessingService.ExecuteAsync(runbookStepExecutionRequest);
}