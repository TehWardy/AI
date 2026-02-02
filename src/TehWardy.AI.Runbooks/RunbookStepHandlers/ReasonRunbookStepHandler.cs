using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Orchestrations;
using TehWardy.AI.Runbooks.RunbookStepHandlers;

namespace TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;

internal class ReasonRunbookStepHandler(
    IReasonRunbookStepHandlerOrchestrationService reasonRunbookStepHandlerOrchestrationService)
        : IRunbookStepHandler
{
    public IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest request) =>
        reasonRunbookStepHandlerOrchestrationService.HandleRunbookStepAsync(request);
}