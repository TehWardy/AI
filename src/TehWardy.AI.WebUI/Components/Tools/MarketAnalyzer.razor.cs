using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Components.Tools;

public partial class MarketAnalyzer : ComponentBase, IToolComponent
{
    [Parameter, EditorRequired]
    public ToolState ToolState { get; set; }

    [Parameter]
    public EventCallback<string> OnToolStateChanged { get; set; }

    public void ApplyAssistantState(string newToolStateJson)
    {
        throw new NotImplementedException();
    }
}