using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Foundations;

internal interface IRunbookStepHandlerService
{
    IAsyncEnumerable<Token> ExecuteStepAsync(
        RunbookStepExecutionRequest runbookStepExecutionRequest);
}