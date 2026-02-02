using TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.RunbookStepHandlers;

internal class LoopingRunbookStepHandler(
    ILoopingRunbookStepHandlerProcessingService loopingRunbookStepHandlerProcessingService)
        : IRunbookStepHandler
{
    public IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest request) =>
            loopingRunbookStepHandlerProcessingService.HandleRunbookStepAsync(request);
}
