namespace TehWardy.AI.Runbooks.Models;

internal class PlannedTask
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PlannedTask[] SubTasks { get; set; }
}