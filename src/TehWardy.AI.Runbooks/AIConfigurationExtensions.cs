using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Agents.Runbooks;
using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Foundations;
using TehWardy.AI.Providers;
using TehWardy.AI.Runbooks;
using TehWardy.AI.Runbooks.Brokers;

namespace TehWardy.AI.Agents;

public static class AIProviderConfigurationExtensions
{
    public static void WithRunbookStepHandlers(
        this AIProviderConfiguration aiConfiguration,
        Action<RunbookStepHandlerConfiguration> stepHandlingConfigurationAction)
    {
        aiConfiguration.ServiceCollection.AddTransient<IParameterParsingBroker, ParameterParsingBroker>();
        aiConfiguration.ServiceCollection.AddTransient<IInferrenceBroker, InferrenceBroker>();
        aiConfiguration.ServiceCollection.AddTransient<IRunbookStepHandlerBroker, RunbookStepHandlerBroker>();
        aiConfiguration.ServiceCollection.AddTransient<IRunbookStepHandlerService, RunbookStepHandlerService>();
        aiConfiguration.ServiceCollection.AddTransient<IRunbookRunnerProcessingService, RunbookRunnerProcessingService>();
        aiConfiguration.ServiceCollection.AddTransient<IRunbookRunner, RunbookRunner>();

        RunbookStepHandlerConfiguration stepConfiguration = new(aiConfiguration);
        stepHandlingConfigurationAction(stepConfiguration);
    }
}