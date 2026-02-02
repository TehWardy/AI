using TehWardy.AI.Agents.Runbooks;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Brokers;

internal class RunbookRunnerBroker(IRunbookRunner runbookRunner)
    : IRunbookRunnerBroker
{
    public IAsyncEnumerable<Token> ExecuteRunbookAsync(RunbookExecutionRequest runbookExecutionRequest) =>
        runbookRunner.ExecuteRunbookRequestAsync(runbookExecutionRequest);
}
