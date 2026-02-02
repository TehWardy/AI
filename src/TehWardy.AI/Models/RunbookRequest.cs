using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Models;

public class RunbookRequest
{
    public Runbook Runbook { get; set; }
    public List<ChatMessage> ConversationHistory { get; set; }
}
