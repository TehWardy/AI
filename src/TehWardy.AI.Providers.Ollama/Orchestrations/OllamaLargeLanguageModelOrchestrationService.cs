using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Foundations;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Orchestrations;

internal class OllamaLargeLanguageModelOrchestrationService(
    IOllamaModelService ollamaModelService,
    IOllamaConversationService ollamaConversationService)
        : IOllamaLargeLanguageModelOrchestrationService
{
    public async IAsyncEnumerable<Token> InferStreamingAsync(
        InferrenceRequest inferrenceRequest)
    {
        IAsyncEnumerable<Token> response =
            EnsureModelIsLoaded(inferrenceRequest.LLMModelName);

        await foreach (var token in response)
            yield return token;

        response = ollamaConversationService
            .InferStreamingAsync(inferrenceRequest);

        await foreach (var token in response)
            yield return token;
    }

    public async ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest)
    {
        IAsyncEnumerable<Token> response =
            EnsureModelIsLoaded(inferrenceRequest.LLMModelName);

        await foreach (var token in response)
            continue;

        return await ollamaConversationService
            .InferAsync(inferrenceRequest);
    }

    async IAsyncEnumerable<Token> EnsureModelIsLoaded(string modelName)
    {
        OllamaModel model = await ollamaModelService
            .GetModelDetailsAsync(modelName);

        if (model is null || model.Details is null)
        {
            yield return new Token
            {
                Thought = $"Loading model '{modelName}' ...\n"
            };

            await ollamaModelService
                .DownloadModelAsync(modelName);

            yield return new Token
            {
                Thought = $"Download complete!\n Executing request ...\n"
            };
        }
    }
}