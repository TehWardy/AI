using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Runbooks.Processings;

internal interface IToolExecutionProcessingService
{
    IAsyncEnumerable<ToolCallExecutionToken> ExecuteCalls(RunbookStepExecutionRequest request, ToolCall[] toolCalls);
}