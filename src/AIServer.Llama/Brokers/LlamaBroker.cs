using AIServer.Llama.Models;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIServer.Llama.Brokers;

internal class LlamaBroker : ILlamaBroker
{
    private LLamaWeights modelWeights;
    private LLamaContext llamaContext;
    private InteractiveExecutor executor;
    private InferenceParams inferenceParams;
    private ChatSession session;
    private string modelName;
    private readonly LlamaConfiguration config;

    public LlamaBroker(LlamaConfiguration config)
    {
        // These seems to be optional 
        inferenceParams = new InferenceParams
        {
            MaxTokens = 1024,
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f,
                TopK = 40
            }
        };
        this.config = config;
    }

    public IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt)
    {
        bool applyInputTransformPipeline =
            session.InputTransformPipeline.Any();

        session.History.Messages.Clear();
        session.History.Messages.AddRange(prompt.History);

        return session.ChatAsync(
            prompt.Message,
            applyInputTransformPipeline,
            inferenceParams);
    }

    public void LoadModel(string modelName)
    {
        this.modelName = modelName;

        var modelParams = new ModelParams(Path.Combine(config.ModelPath, modelName + ".gguf"))
        {
            ContextSize = 4096,
            GpuLayerCount = 128
        };

        this.modelWeights = LLamaWeights.LoadFromFile(modelParams);
        this.llamaContext = modelWeights.CreateContext(modelParams);
        this.executor = new InteractiveExecutor(llamaContext);

        var history = new ChatHistory();
        history.Messages = [];
        this.session = new ChatSession(executor, history);
    }

    public string GetCurrentModelName() => 
        this.modelName;
}