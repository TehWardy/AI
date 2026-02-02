using TehWardy.AI.Agents.Runbooks.Orchestrations;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.RunbookStepHandlers;

internal class ToolCallingRunbookStepHandler(
    IToolCallingRunbookStepHandlerOrchestrationService toolCallingRunbookStepHandlerOrchestrationService)
        : IRunbookStepHandler
{
    public IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest runbookStepExecutionRequest) =>
        toolCallingRunbookStepHandlerOrchestrationService.HandleRunbookStepAsync(runbookStepExecutionRequest);
}
