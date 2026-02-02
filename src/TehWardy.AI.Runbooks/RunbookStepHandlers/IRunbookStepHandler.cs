using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.RunbookStepHandlers;

public interface IRunbookStepHandler
{
    IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest runbookStepExecutionRequest);
}