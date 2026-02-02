using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Agents.Runbooks;
public interface IRunbookRunner
{
    IAsyncEnumerable<Token> ExecuteRunbookRequestAsync(RunbookExecutionRequest runbookExecutionRequest);
}