using TehWardy.AI.Fundations;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Orchestrations;

internal class RunbookOrchestrationService(
    IRunbookService runbookService,
    IRunbookRunnerService runbookRunnerService)
        : IRunbookOrchestrationService
{
    public async IAsyncEnumerable<Token> ExecuteRunbookOrchestrationRequestAsync(
        RunbookOrchestrationRequest runbookOrchestrationRequest)
    {
        string runbookName = runbookOrchestrationRequest.RunbookName;

        Runbook runbook = await runbookService
            .RetrieveRunbookByNameAsync(runbookName);

        var runbookRequest = new RunbookRequest
        {
            Runbook = runbook,
            ConversationHistory = runbookOrchestrationRequest.History
        };

        IAsyncEnumerable<Token> result =
            runbookRunnerService.ExecuteRunbookAsync(runbookRequest);

        await foreach (var token in result)
            yield return token;
    }
}