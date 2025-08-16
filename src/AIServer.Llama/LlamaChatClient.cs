using AIServer.Llama.Foundations;
using AIServer.Llama.Models;
using System.Threading.Tasks;

namespace AIServer.Llama;

public sealed class LlamaChatClient : ILlamaChatClient
{
    private const string SystemPrompt = "You are a concise assistant. keep your answers to user prompts short.";
    private readonly ILlamaService llamaService;

    public LlamaChatClient(ILlamaService llamaService) =>
        this.llamaService = llamaService;

    public async ValueTask InitializeChatSession(string modelName) =>
        await llamaService.InitializeChatSession(modelName, SystemPrompt);

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
