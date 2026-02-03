using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Processings;
internal interface IExecuteRunbookStepProcessingService
{
    IAsyncEnumerable<Token> ExecuteAsync(RunbookStepExecutionRequest runbookStepExecutionRequest);
}