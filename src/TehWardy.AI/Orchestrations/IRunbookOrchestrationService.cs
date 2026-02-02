using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Orchestrations;

internal interface IRunbookOrchestrationService
{
    IAsyncEnumerable<Token> ExecuteRunbookOrchestrationRequestAsync(RunbookOrchestrationRequest runbookOrchestrationRequest);
}