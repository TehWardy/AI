using TehWardy.AI.Agents.Runbooks.Brokers;
using TehWardy.AI.Agents.Runbooks.Orchestrations;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Runbooks.Processings;

namespace TehWardy.AI.Runbooks.Orchestrations;

internal class RespondRunbookStepHandlerOrchestrationService(
    IAccumulationTokenInferrenceProcessingService accumulationTokenInferrenceProcessingService,
    IRunbookStepExecutionResultProcessingService<AccumulatedToken> runbookStepExecutionResultProcessingService,
    IParameterParsingBroker parameterParsingBroker)
        : IRespondRunbookStepHandlerOrchestrationService
{
    public async IAsyncEnumerable<Token> HandleRunbookStepAsync(
        RunbookStepExecutionRequest request)
    {
        InferrenceRequest inferrenceRequest = CreateInferenceRequest(request);

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
            else
                yield return token;
        }

        accumulatedToken.ToolCalls = null;

        await runbookStepExecutionResultProcessingService
            .StoreResultsInExecutionContextAsync(request, accumulatedToken);

        request.RunbookExecutionRequest.ConversationHistory.Add(new ChatMessage
        {
            Role = "assistant",
            Thought = accumulatedToken.Thought,
            Message = accumulatedToken.Content
        });

        request.RunbookState.NextStepName = null;
        request.RunbookState.IsDone = true;
    }

    InferrenceRequest CreateInferenceRequest(
        RunbookStepExecutionRequest request)
    {
        string systemPrompt = CreateSystemPrompt(request);
        List<ChatMessage> messages = CreateMessageHistory(request, systemPrompt);

        bool allowTools = parameterParsingBroker
            .GetBool(request.Step.Parameters, "AllowTools", defaultValue: false);

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

            Tools = allowTools
                ? request.RunbookExecutionRequest.Runbook.Policy.AllowedTools
                : Array.Empty<Tool>()
        };
    }

    private List<ChatMessage> CreateMessageHistory(RunbookStepExecutionRequest request, string systemMessage)
    {
        int maxHistory = parameterParsingBroker
            .GetInt(request.Step.Parameters, "MaxHistoryMessages", defaultValue: 10);

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

    string CreateSystemPrompt(RunbookStepExecutionRequest request)
    {
        string instruction = parameterParsingBroker
            .GetString(request.Step.Parameters, "Instruction")
                ?? "Return the final answer to the user. Be concise and clear.";

        List<string> systemParts = [];

        if (!string.IsNullOrWhiteSpace(request.Step.Purpose))
            systemParts.Add($"Step purpose: {request.Step.Purpose}");

        if (!string.IsNullOrWhiteSpace(instruction))
            systemParts.Add(instruction);

        return string.Join("\n\n", systemParts);
    }
}