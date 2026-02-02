namespace TehWardy.AI.WebUI.Models;

public class ToolState
{
    public string ToolName { get; init; }
    public Guid InstanceId { get; init; }
    public string Title { get; init; }
    public object State { get; init; }
}