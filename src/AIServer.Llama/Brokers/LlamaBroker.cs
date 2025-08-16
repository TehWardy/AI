using AIServer.Llama.Models;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIServer.Llama.Brokers;

internal class LlamaBroker : ILlamaBroker
{
    private InferenceParams inferenceParams;
    private LLamaWeights modelWeights;
    private LLamaContext llamaContext;
    private string modelName;
    private readonly LlamaConfiguration config;

    public LlamaBroker(LlamaConfiguration config)
    {
        this.config = config;

        inferenceParams = new InferenceParams
        {
            MaxTokens = 256,
            AntiPrompts = new List<string> { "User:" },   // use the actual stop token(s) your model emits
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.7f,
                TopP = 0.9f,
                MinP = 0.05f,
                TopK = 40,
                RepeatPenalty = 1.1f
            }
        };
    }

    public IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt)
    {
        ChatSession session = CreateSession(prompt.History);
        return session.ChatAsync(prompt.Message, inferenceParams);
    }

    public void LoadModel(string modelName)
    {
        this.modelName = modelName;
        string modelPath = Path.Combine(config.ModelsPath, modelName + ".gguf");

        var modelParams = new ModelParams(modelPath)
        {
            GpuLayerCount = -1,
            ContextSize = 4096,          
            BatchSize = 128,              
            UseMemorymap = true,
            UseMemoryLock = false
        };

        this.modelWeights = LLamaWeights.LoadFromFile(modelParams);
        this.llamaContext = modelWeights.CreateContext(modelParams);
    }

    public string GetCurrentModelName() => 
        this.modelName;

    ChatSession CreateSession(List<ChatHistory.Message> historyMessages)
    {
        var executor = new InteractiveExecutor(llamaContext);
        var history = new ChatHistory();
        history.Messages = historyMessages;
        return new ChatSession(executor, history);
    }
}