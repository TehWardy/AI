using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TehWardy.AI.WebUI.Foundations;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Components.Conversation;

public partial class Welcome : ComponentBase
{
    public string UserInput { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<Guid> OnUserInput { get; set; }

    protected bool IsSendDisabled => string.IsNullOrWhiteSpace(UserInput);

    [Inject]
    IAgenticConversationService agenticConversationService { get; set; }

    private const string SuggestionCms =
        "Today I'd like to build a content management system. " +
        "Start by proposing an architecture based on common CMS types (pages, with content, users with roles, and link them up." +
        "Model the architecture for the baseline 'CRUD' functions from per type exposure points. " +
        "The bottom of the dependency stack should store the data in a SQL database.";

    private const string SuggestionArchitecture =
        "Help me design a clean software architecture for a .NET backend + Blazor frontend. " +
        "Ask clarifying questions first.";

    private const string SuggestionFtse =
        "Let's analyze the FTSE 100 today. " +
        "Start with a quick market overview and key movers within the index.";

    protected async Task SendAsync()
    {
        if (IsSendDisabled)
            return;

        Prompt prompt = new()
        {
            Input = UserInput
        };

        Models.Conversation conversation =
            await agenticConversationService
                .CreateConversationAsync(prompt);

        await OnUserInput.InvokeAsync(conversation.Id);
    }

    protected async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
            await SendAsync();
    }

    protected async Task OnSuggestionCms()
    {
        UserInput = SuggestionCms;
        await SendAsync();
    }

    protected async Task OnSuggestionArchitecture()
    {
        UserInput = SuggestionArchitecture;
        await SendAsync();
    }

    protected async Task OnSuggestionFtse()
    {
        UserInput = SuggestionFtse;
        await SendAsync();
    }
}
