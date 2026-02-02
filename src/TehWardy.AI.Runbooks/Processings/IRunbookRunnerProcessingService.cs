using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Agents.Runbooks;
internal interface IRunbookRunnerProcessingService
{
    IAsyncEnumerable<Token> ExecuteRunbookRequestAsync(RunbookExecutionRequest runbookExecutionRequest);
}