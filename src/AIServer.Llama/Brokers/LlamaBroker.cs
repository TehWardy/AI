using AIServer.Llama.Models;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIServer.Llama.Brokers;

internal class LlamaBroker : ILlamaBroker
{
    private LlamaConfiguration config;
    private InferenceParams inferenceParams;
    private ChatSession chatSession;

    public LlamaBroker(LlamaConfiguration config) =>
        this.config = config;

    public ValueTask InitializeChatSession(string modelName, string prompt)
    {
        inferenceParams = new InferenceParams
        {
            MaxTokens = 256,
            AntiPrompts = ["User:"],
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.7f,
                TopP = 0.9f,
                MinP = 0.05f,
                TopK = 40,
                RepeatPenalty = 1.1f
            }
        };

        LLamaContext llamaContext = LoadModel(
            Path.Combine(config.ModelsPath, $"{modelName}.gguf"));

        chatSession = CreateSession(llamaContext);
        chatSession.History.AddMessage(AuthorRole.System, prompt);

        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt) =>
        chatSession.ChatAsync(prompt.Message, inferenceParams);

    LLamaContext LoadModel(string modelPath)
    {
        var modelParams = new ModelParams(modelPath)
        {
            GpuLayerCount = -1,
            ContextSize = 4096,
            BatchSize = 128,
            UseMemorymap = true,
            UseMemoryLock = false
        };

        var modelWeights = LLamaWeights.LoadFromFile(modelParams);
        return modelWeights.CreateContext(modelParams);
    }

    static ChatSession CreateSession(LLamaContext llamaContext)
    {
        var executor = new InteractiveExecutor(llamaContext);
        return new ChatSession(executor);
    }
}
