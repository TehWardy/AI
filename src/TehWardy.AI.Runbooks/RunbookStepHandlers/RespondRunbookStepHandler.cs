using TehWardy.AI.Agents.Runbooks.Orchestrations;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.RunbookStepHandlers;

namespace TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;

internal class RespondRunbookStepHandler(
    IRespondRunbookStepHandlerOrchestrationService respondRunbookStepHandlerOrchestrationService)
        : IRunbookStepHandler
{
    public IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest runbookStepExecutionRequest) =>
        respondRunbookStepHandlerOrchestrationService.HandleRunbookStepAsync(runbookStepExecutionRequest);
}