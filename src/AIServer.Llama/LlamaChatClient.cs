using AIServer.Llama.Foundations;
using AIServer.Llama.Models;
using AIServer.Llama.Processings;

namespace AIServer.Llama;

public sealed class LlamaChatClient : ILlamaChatClient
{
    private readonly ILlamaProcessingService llamaService;

    public LlamaChatClient(ILlamaProcessingService llamaService) =>
        this.llamaService = llamaService;

    public async ValueTask InitializeChatSession(string modelName, string systemPrompt)
    {
        await llamaService.InitializeChatSession(
            modelName,
            systemPrompt);
    }

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