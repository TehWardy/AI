using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Components.Conversation;

public partial class Conversation : ComponentBase
{
    [Parameter]
    public Guid ConversationId { get; set; }

    [Inject]
    protected NavigationManager Nav { get; set; }

    protected Chat Chat { get; set; }
    protected ToolHost ToolHost { get; set; }

    protected ToolState ToolState { get; set; }

    protected void HandleNewConversationStart(Guid conversationId) =>
        Nav.NavigateTo($"/Conversation/{conversationId}");

    protected void HandleToolOpenRequested(ToolState nextTool)
    {
        ToolState = nextTool;
        StateHasChanged();
    }

    protected void HandleToolCloseRequested()
    {
        ToolState = null;
        StateHasChanged();
    }

    protected void HandleToolStateChanged()
    {
        Chat.ToolStateJson = ToolState.State is not null 
            ? JsonSerializer.Serialize(ToolState.State)
            : null;
    }

    protected void HandleNewToolStateFromAssistant(string serializedToolState)
    {
        if (ToolState is null)
        { 
            StateHasChanged();
            return;
        }

        ToolHost.AssistantToolStateChanged(serializedToolState);
    }
}