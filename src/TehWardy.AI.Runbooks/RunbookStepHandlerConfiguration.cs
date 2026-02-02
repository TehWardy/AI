using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Foundations;
using TehWardy.AI.Agents.Runbooks.Orchestrations;
using TehWardy.AI.Agents.Runbooks.Processings;
using TehWardy.AI.Agents.Runbooks.RunbookStepHandlers;
using TehWardy.AI.Providers;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Brokers;
using TehWardy.AI.Runbooks.Foundations;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Orchestrations;
using TehWardy.AI.Runbooks.Processings;
using TehWardy.AI.Runbooks.RunbookStepHandlers;

namespace TehWardy.AI.Runbooks;

public class RunbookStepHandlerConfiguration
{
    IServiceCollection serviceCollection { get; }

    internal RunbookStepHandlerConfiguration(AIProviderConfiguration aiConfiguration) =>
        serviceCollection = aiConfiguration.ServiceCollection;

    public void AddReasoningStepHandler(string stepName)
    {
        serviceCollection.AddKeyedTransient<IRunbookStepHandler, ReasonRunbookStepHandler>(stepName);

        serviceCollection.AddTransient<
            IReasonRunbookStepHandlerOrchestrationService,
            ReasonRunbookStepHandlerOrchestrationService>();

        serviceCollection.AddTransient<
            IAccumulationTokenInferrenceProcessingService,
            AccumulationTokenInferrenceProcessingService>();

        serviceCollection.AddTransient<
            IRunbookStepExecutionResultProcessingService<AccumulatedToken>,
            RunbookStepExecutionResultProcessingService<AccumulatedToken>>();

        serviceCollection.AddTransient<
            IAccumulationTokenInferrenceService,
            AccumulationTokenInferrenceService>();

        serviceCollection.AddTransient<
            ISummaryBuilderService<AccumulatedToken>,
            AccumulatedTokenSummaryBuilderService>();
    }

    public void AddRespondingStepHandler(string stepName)
    {
        serviceCollection.AddKeyedTransient<IRunbookStepHandler, RespondRunbookStepHandler>(stepName);

        serviceCollection.AddTransient<
            IRespondRunbookStepHandlerOrchestrationService,
            RespondRunbookStepHandlerOrchestrationService>();

        serviceCollection.AddTransient<
            IAccumulationTokenInferrenceProcessingService,
            AccumulationTokenInferrenceProcessingService>();

        serviceCollection.AddTransient<
            IRunbookStepExecutionResultProcessingService<AccumulatedToken>,
            RunbookStepExecutionResultProcessingService<AccumulatedToken>>();

        serviceCollection.AddTransient<
            IAccumulationTokenInferrenceService,
            AccumulationTokenInferrenceService>();

        serviceCollection.AddTransient<
            ISummaryBuilderService<AccumulatedToken>,
            AccumulatedTokenSummaryBuilderService>();
    }

    public void AddToolCallingStepHandler(string stepName)
    {
        serviceCollection.AddKeyedTransient<IRunbookStepHandler, ToolCallingRunbookStepHandler>(stepName);

        serviceCollection.AddTransient<
            IToolCallingRunbookStepHandlerOrchestrationService,
            ToolCallingRunbookStepHandlerOrchestrationService>();

        serviceCollection.AddTransient<
            IToolExecutionProcessingService,
            ToolExecutionProcessingService>();

        serviceCollection.AddTransient<
            IToolExecutionService,
            ToolExecutionService>();

        serviceCollection.AddTransient<
            IRunbookStepExecutionResultProcessingService<List<ToolExecutionToken>>,
            RunbookStepExecutionResultProcessingService<List<ToolExecutionToken>>>();

        serviceCollection.AddTransient<
            ISummaryBuilderService<List<ToolExecutionToken>>,
            ToolExecutionResultSummaryBuilderService>();

        serviceCollection.AddTransient<IToolInstanceBroker, ToolInstanceBroker>();
    }

    public void AddLoopingStepHandler(string stepName)
    {
        serviceCollection.AddKeyedTransient<IRunbookStepHandler, LoopingRunbookStepHandler>(stepName);

        serviceCollection.AddTransient<
            ILoopingRunbookStepHandlerProcessingService,
            LoopingRunbookStepHandlerProcessingService>();
    }
}