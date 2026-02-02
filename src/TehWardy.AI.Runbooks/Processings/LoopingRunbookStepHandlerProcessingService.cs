using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Foundations;
using TehWardy.AI.Agents.Runbooks.Models;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;

internal class LoopingRunbookStepHandlerProcessingService(
    IParameterParsingBroker parameterParsingBroker,
    IRunbookStepHandlerService runbookStepHandlerService)
        : ILoopingRunbookStepHandlerProcessingService
{
    public async IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest request)
    {
        int maxIterations = parameterParsingBroker
            .GetInt(request.Step.Parameters, "MaxIterations", defaultValue: 5);

        string exitFlagVariableName = parameterParsingBroker
                .GetString(request.Step.Parameters, "ShouldContinue", defaultValue: null);

        int loopNumber = 0;
        bool shouldContinue = true;

        while (loopNumber < maxIterations && shouldContinue)
        {
            IAsyncEnumerable<Token> tokenStream = ExecuteLoopAsync(request);

            await foreach (Token token in tokenStream)
                yield return token;

            shouldContinue = parameterParsingBroker
                .GetBool(request.RunbookState.Variables, exitFlagVariableName, defaultValue: true);

            loopNumber++;
        }

        if (loopNumber == maxIterations)
            yield return new Token { Content = $"Logic Loop Reached Max Iterations = {maxIterations}, Stopped!." };
    }

    async IAsyncEnumerable<Token> ExecuteLoopAsync(RunbookStepExecutionRequest request)
    {
        RunbookStep[] loopSteps = request.Step.Steps;

        foreach (RunbookStep step in loopSteps)
        {
            RunbookStepExecutionRequest stepExecutionRequest = new()
            {
                RunbookExecutionRequest = request.RunbookExecutionRequest,
                RunbookState = request.RunbookState,
                Step = step
            };

            IAsyncEnumerable<Token> handlerResponse =
                runbookStepHandlerService.ExecuteStepAsync(stepExecutionRequest);

            await foreach (var token in handlerResponse)
                yield return token;
        }
    }
}