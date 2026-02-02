using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Brokers;

internal interface IRunbookRunnerBroker
{
    IAsyncEnumerable<Token> ExecuteRunbookAsync(RunbookExecutionRequest runbookExecutionRequest);
}