namespace TehWardy.AI.Agents.Runbooks.Models;

public class RunbookStep
{
    public string Name { get; set; }
    public string NextStepName { get; set; }
    public string Purpose { get; set; }
    public string StepType { get; set; }
    public IDictionary<string, object> Parameters { get; set; }
    public RunbookStep[] Steps { get; set; }
}