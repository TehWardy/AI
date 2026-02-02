using TehWardy.AI.Agents.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Models;

public class Runbook
{
    public string Name { get; set; }
    public string FirstStepName { get; set; }
    public RunbookPolicy Policy { get; set; }
    public RunbookStep[] Steps { get; set; }
}