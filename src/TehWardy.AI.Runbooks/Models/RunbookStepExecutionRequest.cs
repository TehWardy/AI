using TehWardy.AI.Agents.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Models;

public class RunbookStepExecutionRequest
{
    public RunbookExecutionRequest RunbookExecutionRequest { get; set; }
    public RunbookStep Step { get; set; }
    public RunbookState RunbookState { get; set; }
}