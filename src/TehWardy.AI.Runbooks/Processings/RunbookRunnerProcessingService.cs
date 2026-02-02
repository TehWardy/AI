using TehWardy.AI.Agents.Runbooks.Foundations;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks;

internal class RunbookRunnerProcessingService(IRunbookStepHandlerService runbookStepHandlerService)
    : IRunbookRunnerProcessingService
{
    public async IAsyncEnumerable<Token> ExecuteRunbookRequestAsync(RunbookExecutionRequest runbookExecutionRequest)
    {
        RunbookState runbookState =
            CreateInitialRunbookState(runbookExecutionRequest.Runbook.FirstStepName);

        while (RunbookStillRunning(runbookState))
        {
            RunbookStepExecutionRequest runbookStepExecutionRequest =
                CreateRunbookStepExecutionRequest(runbookExecutionRequest, runbookState);

            if (runbookStepExecutionRequest.Step is null)
            {
                runbookState.IsDone = true;

                yield return new Token
                {
                    Thought = $"Runbook appears to be broken: step '{runbookState.NextStepName}' not found.\n Stopping Execution."
                };

                yield break;
            }

            IAsyncEnumerable<Token> handlerResponse =
                runbookStepHandlerService.ExecuteStepAsync(runbookStepExecutionRequest);

            await foreach (var token in handlerResponse)
                yield return token;
        }

        if (runbookState.StepExecutions >= 50)
        {
            yield return new Token
            {
                Thought = "Runbook execution terminated: maximum step count reached."
            };
        }
    }

    static bool RunbookStillRunning(RunbookState runbookState) =>
        !runbookState.IsDone
        && runbookState.NextStepName is not null
        && runbookState.StepExecutions < 50;

    static RunbookStepExecutionRequest CreateRunbookStepExecutionRequest(
        RunbookExecutionRequest runbookExecutionRequest,
        RunbookState runbookState)
    {
        RunbookStep currentStep = runbookExecutionRequest.Runbook.Steps
            .FirstOrDefault(step => step.Name == runbookState.NextStepName);

        return new()
        {
            RunbookExecutionRequest = runbookExecutionRequest,
            Step = currentStep,
            RunbookState = runbookState
        };
    }


    static RunbookState CreateInitialRunbookState(string firstStepName) => new()
    {
        IsDone = firstStepName is null,
        NextStepName = firstStepName,
        Variables = new Dictionary<string, object>(),
    };
}