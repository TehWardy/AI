using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Brokers;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Fundations;

internal class RunbookRunnerService(IRunbookRunnerBroker runbookRunnerBroker)
    : IRunbookRunnerService
{
    public IAsyncEnumerable<Token> ExecuteRunbookAsync(RunbookRequest runbookRequest)
    {
        RunbookExecutionRequest runbookExecutionRequest =
            MapToRunbookExecutionRequest(runbookRequest);

        return runbookRunnerBroker.ExecuteRunbookAsync(runbookExecutionRequest);
    }

    static RunbookExecutionRequest MapToRunbookExecutionRequest(RunbookRequest runbookRequest) => new()
    {
        Runbook = runbookRequest.Runbook,
        ConversationHistory = runbookRequest.ConversationHistory
    };
}