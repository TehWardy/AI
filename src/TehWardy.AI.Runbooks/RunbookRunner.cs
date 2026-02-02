using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Agents.Runbooks;

internal class RunbookRunner(IRunbookRunnerProcessingService runbookRunnerProcessingService)
    : IRunbookRunner
{
    public IAsyncEnumerable<Token> ExecuteRunbookRequestAsync(
        RunbookExecutionRequest runbookExecutionRequest)
    {
        return runbookRunnerProcessingService
            .ExecuteRunbookRequestAsync(runbookExecutionRequest);
    }
}
