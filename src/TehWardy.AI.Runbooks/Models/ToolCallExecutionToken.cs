using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Models;

internal class ToolCallExecutionToken
{
    public string Content { get; set; }
    public ToolExecutionToken Result { get; set; }
}