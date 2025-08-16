using AIServer.Llama.Foundations;
using AIServer.Llama.Models;

namespace AIServer.Llama;

public sealed class LlamaChatClient : ILlamaChatClient
{
    private readonly ILlamaService llamaService;

    public LlamaChatClient(ILlamaService llamaService) =>
        this.llamaService = llamaService;

    public IAsyncEnumerable<string> InitializeChatSession(string modelName) =>
        llamaService.InitializeChatSession(modelName);

    public IAsyncEnumerable<string> SendAsync(string userMessage)
    {
        var prompt = new ChatPrompt
        {
            Message = new MessageData
            {
                Role = "user",
                Content = userMessage
            }
        };

        return llamaService.SendPromptAsync(prompt);
    }
}