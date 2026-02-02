using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Orchestrations;
internal interface IRespondRunbookStepHandlerOrchestrationService
{
    IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest request);
}