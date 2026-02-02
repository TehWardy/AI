using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TehWardy.AI.WebUI.Foundations;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Components.Conversation;

public partial class Chat : ComponentBase
{
    [Inject]
    IAgenticConversationService agenticConversationService { get; set; }

    public string ToolStateJson { get; set; }
    public string UserInput { get; set; }
    public bool IsProcessing { get; set; } = true;

    [Parameter]
    public Guid ConversationId { get; set; }

    [Parameter]
    public EventCallback<ToolState> OnToolOpenRequested { get; set; }

    [Parameter]
    public EventCallback OnToolCloseRequested { get; set; }

    [Parameter]
    public EventCallback<string> OnNewToolStateFromAssistant  {get; set; }

    Models.Conversation Conversation { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (ConversationId != Guid.Empty)
        {
            Conversation = await agenticConversationService
                .RetrieveConversationAsync(ConversationId);

            if (Conversation.History.Last().Role == "user")
                await ResumeAsync();
        }
    }

    async ValueTask ResumeAsync()
    {
        Prompt prompt = new()
        {
            Input = Conversation.History.Last().Message,
            ConversationId = ConversationId
        };

        IAsyncEnumerable<Token> response = agenticConversationService
            .SendPromptAsync(prompt);

        await HandleAiResponse(response);
    }

    async Task SendAsync()
    {
        if (IsProcessing)
            return;

        Prompt prompt = new()
        {
            Input = ToolStateJson is not null 
                ? $"{UserInput}\n{ToolStateJson}" 
                : UserInput,
            ConversationId = ConversationId
        };

        IAsyncEnumerable<Token> response = agenticConversationService
            .SendPromptAsync(prompt);

        await HandleAiResponse(response);
    }

    async ValueTask HandleAiResponse(IAsyncEnumerable<Token> response)
    {
        ChatMessage message = new()
        {
            Role = "assistant"
        };

        Conversation.History.Add(message);

        await foreach (var token in response)
        {
            if (token.Thought == "UI Tool Recommendation")
            {
                ToolState toolState = new()
                {
                    Title = token.Content,
                    ToolName = token.Content,
                    InstanceId = Guid.NewGuid()
                };

                await OnToolOpenRequested.InvokeAsync(toolState);
            }
            else if (token.Thought == "Tool State Update")
            {
                await OnNewToolStateFromAssistant.InvokeAsync(token.Content);
            }
            else
            {
                message.Thought += token.Thought;
                message.Message += token.Content;
                StateHasChanged();
            }
        }

        UserInput = string.Empty;
        IsProcessing = false;
        StateHasChanged();
    }

    async Task OpenToolStub()
    {
        var tool = new ToolState
        {
            ToolName = "ArchitectureDesigner",
            InstanceId = Guid.NewGuid(),
            Title = "Architecture Designer",
            State = new { }
        };

        await OnToolOpenRequested.InvokeAsync(tool);
    }

    async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
            await SendAsync();
    }
}