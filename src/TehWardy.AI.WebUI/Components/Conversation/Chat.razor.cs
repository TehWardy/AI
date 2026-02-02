using System.Text.Json;
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
                if (TryExtractToolStateJson(token.Content, out string toolStateJson))
                {
                    await OnNewToolStateFromAssistant.InvokeAsync(toolStateJson);
                }
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

    private static bool TryExtractToolStateJson(string content, out string toolStateJson)
    {
        toolStateJson = null;

        if (string.IsNullOrWhiteSpace(content))
            return false;

        string trimmed = StripCodeFences(content.Trim());

        if (TryNormalizeJson(trimmed, out toolStateJson))
            return true;

        int startIndex = trimmed.IndexOf('{');

        if (startIndex < 0)
            return false;

        int depth = 0;
        bool inString = false;
        bool escaped = false;
        int endIndex = -1;

        for (int i = startIndex; i < trimmed.Length; i++)
        {
            char current = trimmed[i];

            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (current == '\\' && inString)
            {
                escaped = true;
                continue;
            }

            if (current == '"')
            {
                inString = !inString;
                continue;
            }

            if (!inString)
            {
                if (current == '{')
                {
                    depth++;
                }
                else if (current == '}')
                {
                    depth--;

                    if (depth == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }
        }

        if (endIndex < 0)
            return false;

        string jsonCandidate = trimmed.Substring(startIndex, endIndex - startIndex + 1);

        return TryNormalizeJson(jsonCandidate, out toolStateJson);
    }

    private static string StripCodeFences(string content)
    {
        if (!content.StartsWith("```"))
            return content;

        int firstNewline = content.IndexOf('\n');

        if (firstNewline < 0)
            return content;

        string withoutOpeningFence = content[(firstNewline + 1)..];

        int closingFenceIndex = withoutOpeningFence.LastIndexOf("```", StringComparison.Ordinal);

        if (closingFenceIndex < 0)
            return withoutOpeningFence.Trim();

        return withoutOpeningFence[..closingFenceIndex].Trim();
    }

    private static bool TryNormalizeJson(string candidate, out string normalizedJson)
    {
        normalizedJson = null;

        try
        {
            using JsonDocument document = JsonDocument.Parse(candidate);
            normalizedJson = JsonSerializer.Serialize(document.RootElement);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
