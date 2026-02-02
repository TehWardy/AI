namespace TehWardy.AI.Agents.Runbooks.Models;

public class RunbookState
{
    public string NextStepName { get; set; }
    public bool IsDone { get; set; }
    public int StepExecutions { get; set; }
    public IDictionary<string, object> Variables { get; set; }
}