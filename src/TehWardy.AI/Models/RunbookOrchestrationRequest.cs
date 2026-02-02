using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Models;

internal class RunbookOrchestrationRequest
{
    public string AgentName { get; set; } = "default";
    public string RunbookName { get; set; } = "Default";
    public Guid ConversationId { get; set; }
    public string Input { get; set; }
    public List<ChatMessage> History { get; set; }
}