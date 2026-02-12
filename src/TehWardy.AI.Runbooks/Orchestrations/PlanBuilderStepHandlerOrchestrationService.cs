using System.Diagnostics;
using System.Text.Json;
using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Processings;

namespace TehWardy.AI.Runbooks.Orchestrations;

internal class PlanBuilderStepHandlerOrchestrationService(
    IAccumulationTokenInferrenceProcessingService accumulationTokenInferrenceProcessingService,
    IRunbookStepExecutionResultProcessingService<PlannedTask> runbookStepExecutionResultProcessingService,
    IParameterParsingBroker parameterParsingBroker) 
        : IPlanBuilderStepHandlerOrchestrationService
{
    public async IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest request)
    {
        PlannedTask plan = new()
        {
            Id = "0",
            Name = "User Prompt",

            Description = request.RunbookExecutionRequest
                .ConversationHistory
                .Last()
                .Message
        };

        PlannedTask currentTask = plan;

        do
        {
            InferrenceRequest inferrenceRequest = CreateInferenceRequest(request, plan, currentTask);

            IAsyncEnumerable<Token> responseTokens = accumulationTokenInferrenceProcessingService
                .SendInferrenceRequestAsync(inferrenceRequest);

            AccumulatedToken accumulatedToken = null;

            await foreach (var token in responseTokens)
            {
                if (token is AccumulatedToken finalToken)
                {
                    accumulatedToken = finalToken;
                    continue;
                }
                else if (token.Thought is not null)
                    yield return token;
            }

            Debug.WriteLine($"[inferrence response]\n{JsonSerializer.Serialize(accumulatedToken)}");

            currentTask.SubTasks = ParsePlan(currentTask, accumulatedToken.Content);
        }
        while (currentTask.SubTasks.Length > 1);

        await runbookStepExecutionResultProcessingService
            .StoreResultsInExecutionContextAsync(request, plan);
    }

    InferrenceRequest CreateInferenceRequest(
        RunbookStepExecutionRequest request, PlannedTask root, PlannedTask currentTask)
    {
        string systemPrompt = CreateSystemPrompt(request, root, currentTask);
        List<ChatMessage> messages = CreateMessageHistory(request, systemPrompt);

        return new InferrenceRequest
        {
            Context = messages,

            LLMProviderName = parameterParsingBroker
                .GetString(request.Step.Parameters, "LLMProviderName"),

            LLMModelName = parameterParsingBroker
                .GetString(request.Step.Parameters, "LLMModelName"),

            EmbeddingProviderName = parameterParsingBroker
                .GetString(request.Step.Parameters, "EmbeddingProviderName"),

            EmbeddingModelName = parameterParsingBroker
                .GetString(request.Step.Parameters, "EmbeddingModelName"),

            MemoryProviderName = parameterParsingBroker
                .GetString(request.Step.Parameters, "MemoryProviderName"),

            ContextLength = parameterParsingBroker
                .GetInt(request.Step.Parameters, "ContextLength", defaultValue: 4096),

            Tools = Array.Empty<Tool>()
        };
    }

    private List<ChatMessage> CreateMessageHistory(RunbookStepExecutionRequest request, string systemMessage)
    {
        int maxHistory = parameterParsingBroker
            .GetInt(request.Step.Parameters, "MaxHistoryMessages", defaultValue: 3);

        IList<ChatMessage> history = request.RunbookExecutionRequest.ConversationHistory;

        IEnumerable<ChatMessage> trimmed = history.Count <= maxHistory
            ? history
            : history.Skip(history.Count - maxHistory);

        List<ChatMessage> messages =
        [
            new() { Role = "system", Message = systemMessage },
            .. trimmed.Where(message => message.Role != "system")
        ];

        return messages;
    }

    static string CreateSystemPrompt(RunbookStepExecutionRequest request, PlannedTask root, PlannedTask currentTask)
    {
        string instruction = """
            You're in the planning phase of a users question. 
            Take the users prompt and break it down in to a series of one or more tasks that can be more easily acheived than the given initial prompt.
            Output a task list as JSON, the schema for which is as follows ...

            { Name: string, Description: string }

            ... if the given task scope looks like it can't be broken down further then just create a single new task containing the given task request.

            The response MUST always be a JSON array of tasks as defined in the schema with at least 1 entry in it.

            The plan so far is ...

            """ + SerializePlan(root) + $"\n\nYou're working on task {currentTask.Id}: {currentTask.Name} ...\n{currentTask.Description}";

        Debug.WriteLine(SerializePlan(root));

        List<string> systemParts = [];

        if (!string.IsNullOrWhiteSpace(request.Step.Purpose))
            systemParts.Add($"Step purpose: {request.Step.Purpose}");

        if (!string.IsNullOrWhiteSpace(instruction))
            systemParts.Add(instruction);

        return string.Join("\n\n", systemParts);
    }

    static PlannedTask[] ParsePlan(PlannedTask parent, string planJson)
    {
        try
        {
            PlannedTask[] subTasks = JsonSerializer
                .Deserialize<PlannedTask[]>(planJson);

            for (int i = 0; i < subTasks.Length; i++)
                subTasks[i].Id = parent.Id + $".{i}";

            return subTasks;
        }
        catch
        {
            return [];
        }
    }

    static string SerializePlan(PlannedTask plan, int depth = 0)
    {
        string depthPrefix = string.Join("", Enumerable.Range(0, depth).Select(i => " "));

        string subplan = plan.SubTasks is not null
            ? string.Join("\n", plan.SubTasks.Select(sub => SerializePlan(sub, depth + 1)))
            : string.Empty;

        return $"{depthPrefix}- {plan.Id}: {plan.Name} {subplan}";
    }
}