using AIServer.Llama.Foundations;
using AIServer.Llama.Models;

namespace AIServer.Llama;

public sealed class LlamaChatClient : ILlamaChatClient
{
    private readonly ILlamaService llamaService;

    public string ModelName { get; set; }
    public List<MessageData> history { get; set; } = [];

    public LlamaChatClient(ILlamaService llamaService)
    {
        this.llamaService = llamaService;
        AddSystemMessage();
    }

    public void LoadModel(string modelName)
    {
        ModelName = modelName;
        llamaService.LoadModel(modelName);
    }

    public void AddSystemMessage()
    {
        var systemPrompt = new MessageData
        {
            Role = "system",
            Content = $"You are a concise and helpful assistant."
        };

        history.Add(systemPrompt);
    }

    public IAsyncEnumerable<string> SendAsync(string userMessage)
    {
        var prompt = new ChatPrompt
        {
            Model = ModelName,
            History = history,

            Message = new MessageData
            {
                Role = "user",
                Content = userMessage
            }
        };

        return llamaService.SendPromptAsync(prompt);
    }
}