using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal class ExecuteRunbookStepProcessingService(IServiceProvider serviceProvider) 
    : IExecuteRunbookStepProcessingService
{
    public IAsyncEnumerable<Token> ExecuteAsync(RunbookStepExecutionRequest runbookStepExecutionRequest)
    {
        Func<RunbookStepExecutionRequest, IServiceProvider, IAsyncEnumerable<Token>> execution =
            runbookStepExecutionRequest.Step.Parameters["Expression"] as
                Func<RunbookStepExecutionRequest, IServiceProvider, IAsyncEnumerable<Token>>;

        return execution(runbookStepExecutionRequest, serviceProvider);
    }
}
