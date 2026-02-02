using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Models;

public class RunbookExecutionRequest
{
    public Runbook Runbook { get; set; }
    public IList<ChatMessage> ConversationHistory { get; set; }
}