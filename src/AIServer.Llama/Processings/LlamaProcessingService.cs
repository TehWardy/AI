using AIServer.Llama.Foundations;
using AIServer.Llama.Models;

namespace AIServer.Llama.Processings;

internal class LlamaProcessingService : ILlamaProcessingService
{
    private readonly ILlamaService llamaService;

    public LlamaProcessingService(ILlamaService llamaService) =>
        this.llamaService = llamaService;

    public async IAsyncEnumerable<string> SendPromptAsync(ChatPrompt prompt)
    {
        IAsyncEnumerable<string> result =
            llamaService.SendPromptAsync(prompt);

        List<string> tokenCache = [];
        string batchString = string.Empty;

        bool firstBatch = true;

        await foreach (string token in result)
        {
            tokenCache.Add(token);

            if (tokenCache.Count > 10)
            {
                batchString = firstBatch 
                    ? string.Concat(tokenCache).Trim()
                    : string.Concat(tokenCache);

                yield return RemoveInternals(batchString, firstBatch).TrimEnd();

                tokenCache.Clear();
                firstBatch = false;
            }
        }

        yield return RemoveInternals(string.Concat(tokenCache), firstBatch);
    }

    string RemoveInternals(string batchString, bool isFirstBatch)
    {
        if (isFirstBatch && batchString.StartsWith("Assistant:"))
            batchString = batchString.Replace("Assistant:", "");

        if (batchString.Contains("<|assistant|>"))
            batchString = batchString.Replace("<|assistant|>", "");

        return batchString;
    }

    public async ValueTask InitializeChatSession(string modelName, string systemPrompt) =>
        await llamaService.InitializeChatSession(modelName, systemPrompt);
}