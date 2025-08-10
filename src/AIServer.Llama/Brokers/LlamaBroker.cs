using LLama;
using LLama.Common;

namespace AIServer.Llama.Brokers;

internal class LlamaBroker : ILlamaBroker
{
    private readonly LLamaWeights modelWeights;
    private readonly LLamaContext llamaContext;
    private readonly InteractiveExecutor executor;
    private readonly InferenceParams inferenceParams;
    private readonly ChatSession session;
    private readonly ChatHistory history;

    public LlamaBroker(string modelName)
    {
        var modelParams = new ModelParams(modelName)
        {
            ContextSize = 4096,
            GpuLayerCount = 128
        };

        modelWeights = LLamaWeights.LoadFromFile(modelParams);
        llamaContext = modelWeights.CreateContext(modelParams);
        executor = new InteractiveExecutor(llamaContext);
        history = new ChatHistory();
        history.Messages = [];
        session = new ChatSession(executor, history);
    }

    public IAsyncEnumerable<string> SendAsync(string userMessage)
    {
        bool applyInputTransformPipeline =
            session.InputTransformPipeline.Any();

        var message = new ChatHistory.Message(
            AuthorRole.User,
            userMessage);

        return session.ChatAsync(
            message,
            applyInputTransformPipeline,
            inferenceParams);
    }
}