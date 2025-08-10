using System.Text.Json;
using System.Text.Json.Nodes;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIServer.Llama;

public sealed class LlamaChat 
{
    private readonly LLamaWeights modelWeights;
    private readonly LLamaContext llamaContext;
    private readonly InteractiveExecutor executor;
    private readonly InferenceParams inferenceParams;
    private readonly ChatSession session;

    private readonly ChatHistory history;

    public LlamaChat(
        string modelName,
        List<ChatHistory.Message> conversationHistory)
    {
        var mp = new ModelParams(modelName)
        {
            ContextSize = 4096,
            GpuLayerCount = 128
        };

        modelWeights = LLamaWeights.LoadFromFile(mp);
        llamaContext = modelWeights.CreateContext(mp);
        executor = new InteractiveExecutor(llamaContext);
        history = new ChatHistory();
        history.Messages = conversationHistory ?? [];
        session = new ChatSession(executor, history);

        AddSystemMessage();

        // DeepSeek / Qwen style tags – provide your own IHistoryTransform if you need it
        //session.WithHistoryTransform(new DeepSeekHistoryTransform());

        inferenceParams = new InferenceParams
        {
            MaxTokens = 1024,
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f,
                TopK = 40
            }
        };
    }

    public void AddSystemMessage()
    {
        history.AddMessage(AuthorRole.System, $"You are a concise and helpful assistant.");
    }

    public async IAsyncEnumerable<string> SendAsync(string userMessage)
    {
        string relevantMemorisedDetails =
            await QueryMemoryForContext(userMessage);

        var message = new ChatHistory.Message(
            AuthorRole.User,
            $"{relevantMemorisedDetails}\n{userMessage}");

        IAsyncEnumerable<string> llmResponse = session.ChatAsync(
            message,
            session.InputTransformPipeline.Any(),
            inferenceParams);

        string responseFragment = string.Empty;
        bool possibleToolResponse = false;

        await foreach (var tok in llmResponse)
        {
            if (tok.Contains("{"))
                possibleToolResponse = true;

            if (possibleToolResponse)
                responseFragment += tok;

            if (!possibleToolResponse && responseFragment.Length > 0)
            {
                (string tool, JsonNode toolArgs) = TryParseMCPCall(responseFragment);

                if (tool is not null && toolArgs is not null)
                {
                    possibleToolResponse = false;
                    string toolResponse = await CallToolAsync(tool, toolArgs);
                    responseFragment = string.Empty;
                    yield return $"Executing Tool:\n{toolResponse}";
                }

                if (responseFragment.Contains("}"))
                {
                    possibleToolResponse = false;
                    string fragmentCopy = responseFragment;
                    responseFragment = string.Empty;
                    yield return fragmentCopy;
                }
            }
            else if (!possibleToolResponse)
                yield return tok;
        }
    }

    async ValueTask<string> QueryMemoryForContext(string userMessage, int topK = 3)
    {
        if (MemoryProvider is null)
            return string.Empty;

        var docs = await MemoryProvider
            .GetRelevantDocumentsAsync(userMessage, maxResults: 3);

        if (docs.Length > 0)
        {
            return "\nRelevant Memory:\n" + string
                .Join("\n", docs.Select((d, i) => $"{i + 1}. {d}"));
        }
        else
            return string.Empty;
    }

    async ValueTask<string> CallToolAsync(string url, JsonNode args)
    {
        HttpRequestDetails details = JsonSerializer
            .Deserialize<HttpRequestDetails>(args.ToJsonString());

        using var client = new HttpClient();

        HttpRequestMessage request =
            new(HttpMethod.Get, details.Url);

        if (details.Headers is not null)
        {
            foreach (var header in details.Headers)
                request.Headers.Add(header.Key, header.Value);
        }

        HttpResponseMessage response =
            await client.SendAsync(request);

        return await response.Content
            .ReadAsStringAsync();
    }

    private static (string, JsonNode) TryParseMCPCall(string text)
    {
        string name = null;
        JsonNode arguments = null;

        // look for a JSON block in the output
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start < 0 || end < start)
            return (name, arguments);

        var jsonPart = text.Substring(start, end - start + 1);

        try
        {
            var node = JsonNode.Parse(jsonPart);

            // MCP instructions look like: { "name": "...", "arguments": { … } }
            name = node?["name"]?.GetValue<string>();
            arguments = node?["arguments"];
        }
        catch { }

        return (name, arguments);
    }
}