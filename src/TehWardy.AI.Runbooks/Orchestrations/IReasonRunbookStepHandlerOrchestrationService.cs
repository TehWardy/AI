using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Orchestrations;

internal interface IReasonRunbookStepHandlerOrchestrationService
{
    IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest request);
}