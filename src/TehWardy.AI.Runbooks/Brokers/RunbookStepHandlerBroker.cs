using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Brokers;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.RunbookStepHandlers;

namespace TehWardy.AI.Agents.Runbooks.Brokers;

internal class RunbookStepHandlerBroker(IServiceProvider serviceProvider)
    : IRunbookStepHandlerBroker
{
    public IAsyncEnumerable<Token> ExecuteRunbookStepAsync(
        RunbookStepExecutionRequest runbookStepExecutionRequest)
    {
        string stepType = runbookStepExecutionRequest
            .Step.StepType;

        IRunbookStepHandler handler = serviceProvider
            .GetRequiredKeyedService<IRunbookStepHandler>(stepType);

        return handler.HandleRunbookStepAsync(runbookStepExecutionRequest);
    }
}