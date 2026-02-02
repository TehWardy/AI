using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;
internal interface ILoopingRunbookStepHandlerProcessingService
{
    IAsyncEnumerable<Token> HandleRunbookStepAsync(RunbookStepExecutionRequest request);
}