using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Brokers;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.Foundations;

internal class RunbookStepHandlerService(IRunbookStepHandlerBroker runbookStepHandlerBroker)
    : IRunbookStepHandlerService
{
    public async IAsyncEnumerable<Token> ExecuteStepAsync(
        RunbookStepExecutionRequest runbookStepExecutionRequest)
    {
        IAsyncEnumerable<Token> handlerResponse = runbookStepHandlerBroker
                .ExecuteRunbookStepAsync(runbookStepExecutionRequest);

        await foreach (var token in handlerResponse)
            yield return token;

        UpdateExecutionState(runbookStepExecutionRequest);
    }

    void UpdateExecutionState(RunbookStepExecutionRequest runbookStepExecutionRequest)
    {
        RunbookState runbookState =
            runbookStepExecutionRequest.RunbookState;

        runbookState.StepExecutions++;

        runbookState.NextStepName =
            runbookStepExecutionRequest.Step?.NextStepName;

        if (runbookState.NextStepName is null)
            runbookState.IsDone = true;
    }
}