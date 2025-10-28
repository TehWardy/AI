using AIServer.Llama.Configurations;
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
            MaxTokens = -1,
            AntiPrompts = ["<|assistant|>", "<|user|>", "<|end|>", "User:"],
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

        chatSession = CreateSession(llamaContext, prompt);

        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt) =>
        chatSession.ChatAsync(prompt.Message, inferenceParams);

    LLamaContext LoadModel(string modelPath)
    {
        var modelParams = new ModelParams(modelPath)
        {
            GpuLayerCount = 128,
            ContextSize = 4096,
            BatchSize = 128,
            UseMemorymap = true,
            UseMemoryLock = false
        };

        var modelWeights = LLamaWeights.LoadFromFile(modelParams);
        return modelWeights.CreateContext(modelParams);
    }

    static ChatSession CreateSession(LLamaContext llamaContext, string systemPrompt)
    {
        var history = new ChatHistory 
        { 
            Messages = new List<ChatHistory.Message> 
            { 
                new ChatHistory.Message(AuthorRole.System, systemPrompt)
            } 
        };

        var executor = new InteractiveExecutor(llamaContext);
        return new ChatSession(executor, history);
    }
}
