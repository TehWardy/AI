using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Components.Tools;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Components.Conversation;

public partial class ToolHost : ComponentBase
{
    [Inject] 
    public IToolUiRegistry ToolRegistry { get; set; }

    [Parameter] 
    public ToolState ToolState { get; set; }

    [Parameter] 
    public EventCallback OnClose { get; set; }

    [Parameter] 
    public EventCallback<string> OnToolStateChanged { get; set; }

    protected string Title => ToolState?.Title ?? ToolState?.ToolName ?? "Tool";

    protected Type ToolComponentType => ToolState is null 
        ? null 
        : ToolRegistry.ResolveComponentType(ToolState.ToolName);

    protected IDictionary<string, object> ToolParameters =>
        ToolState is null ? [] : new Dictionary<string, object>
        {
            ["ToolState"] = ToolState,
            ["OnToolStateChanged"] = OnToolStateChanged
        };

    private IToolComponent activeTool; 

    public void AssistantToolStateChanged(string newToolStateJson)
    {
        if (activeTool is not null)
            activeTool.ApplyAssistantState(newToolStateJson);
    }

    private Task CaptureToolInstance(IToolComponent tool)
    {
        activeTool = tool;
        return Task.CompletedTask;
    }
}
