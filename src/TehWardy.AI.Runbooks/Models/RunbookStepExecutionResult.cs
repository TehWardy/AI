using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Models;

internal class RunbookStepExecutionResult
{
    public RunbookStepExecutionRequest ExecutionRequest { get; set; }
    public AccumulatedToken ExecutionResult { get; set; }
}
