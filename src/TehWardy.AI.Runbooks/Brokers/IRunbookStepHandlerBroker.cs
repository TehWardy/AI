using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Brokers;

internal interface IRunbookStepHandlerBroker
{
    IAsyncEnumerable<Token> ExecuteRunbookStepAsync(
        RunbookStepExecutionRequest runbookStepExecutionRequest);
}