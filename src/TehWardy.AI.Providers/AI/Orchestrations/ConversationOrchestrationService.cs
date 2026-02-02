using TehWardy.AI.Providers.AI.Foundations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Orchestrations;

internal class ConversationOrchestrationService(
    IEmbeddingService embeddingService,
    ILargeLanguageModelService largeLanguageModelService,
    IMemoryService memoryService)
        : IConversationOrchestrationService
{
    public async IAsyncEnumerable<Token> InferStreamingAsync(
        InferrenceRequest inferrenceRequest)
    {
        EmbeddingRequest embeddingRequest =
            CreateEmbeddingRequest(inferrenceRequest);

        float[] embedding = await embeddingService
            .EmbedAsync(embeddingRequest);

        EmbeddedSearchRequest embeddedSearchRequest =
            CreateEmbeddedSearchRequest(
                inferrenceRequest,
                embedding);

        List<RagDocument> context = await memoryService
            .SearchAsync(embeddedSearchRequest);

        AddContextToInferrenceRequest(inferrenceRequest, context);

        IAsyncEnumerable<Token> response = largeLanguageModelService
            .InferStreamingAsync(inferrenceRequest);

        await foreach (Token token in response)
            yield return token;
    }

    static void AddContextToInferrenceRequest(
        InferrenceRequest inferrenceRequest,
        List<RagDocument> context)
    {
        if (context.Count > 0)
        {
            string memorisedContextString =
                string.Join("\n", context.Select(doc => doc.Content));

            ChatMessage systemContextMessage = new()
            {
                Role = "system",
                Message = "The following context is provided to assist you in answering the user's query:\n" +
                    memorisedContextString
            };

            inferrenceRequest.Context = inferrenceRequest.Context
                .Prepend(systemContextMessage)
                .ToList();
        }
    }

    static EmbeddingRequest CreateEmbeddingRequest(
        InferrenceRequest inferrenceRequest) => new()
        {
            ProviderName = inferrenceRequest.EmbeddingProviderName,
            ModelName = inferrenceRequest.EmbeddingModelName,
            Input = inferrenceRequest.Context.Last().Message
        };

    static EmbeddedSearchRequest CreateEmbeddedSearchRequest(
        InferrenceRequest inferrenceRequest,
        float[] embedding) => new()
        {
            Embedding = embedding,
            ProviderName = inferrenceRequest.MemoryProviderName,
            TopK = 3
        };
}
