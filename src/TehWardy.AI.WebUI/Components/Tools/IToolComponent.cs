using Microsoft.AspNetCore.Components;

namespace TehWardy.AI.WebUI.Components.Tools;

public interface IToolComponent : IComponent
{
    void ApplyAssistantState(string newToolStateJson);

    /// Optional: Tool can tell host it changed state (user edits) so host can persist/sync.
    EventCallback<string> OnToolStateChanged { get; set; }
}

public interface IToolUiRegistry
{
    Type ResolveComponentType(string toolName);
}

public sealed class ToolUiRegistry : IToolUiRegistry
{
    private readonly Dictionary<string, Type> map =
        new(StringComparer.OrdinalIgnoreCase);

    public ToolUiRegistry Register<TComponent>(string toolName)
        where TComponent : IToolComponent
    {
        map[toolName] = typeof(TComponent);
        return this;
    }

    public Type ResolveComponentType(string toolName) => 
        toolName != null && map.TryGetValue(toolName, out var t) 
            ? t 
            : null;
}
