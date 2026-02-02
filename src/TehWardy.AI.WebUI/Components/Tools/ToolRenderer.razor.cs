using Microsoft.AspNetCore.Components;

namespace TehWardy.AI.WebUI.Components.Tools;

public partial class ToolRenderer : ComponentBase
{
    [Parameter, EditorRequired] 
    public Type ToolType { get; set; }

    [Parameter, EditorRequired] 
    public IDictionary<string, object> Parameters { get; set; }

    [Parameter] 
    public EventCallback<IToolComponent> OnToolInstanceAvailable { get; set; }

    private DynamicComponent inner;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // DynamicComponent exposes Instance in .NET 8+, but not always surfaced in IntelliSense.
            // If available in your target, this compiles:
            var instance = inner.Instance as IToolComponent;

            if (instance is not null)
                await OnToolInstanceAvailable.InvokeAsync(instance);
        }
    }
}
