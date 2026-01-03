using AIServer.Ollama.Models;
using AIServer.Ollama.Orchestrations;

namespace AIServer.Ollama;

public class OllamaChatClient : IOllamaChatClient
{
    private readonly IOllamaConversationOrchestrationService converstionService;

    public string ModelId { get; set; }
    public List<MessageData> history { get; set; } = [];

    public OllamaChatClient(IOllamaConversationOrchestrationService converstionService) =>
        this.converstionService = converstionService;

    public IAsyncEnumerable<ResponseToken> SendAsync(string userMessage)
    {
        if (userMessage is not null)
            history.Add(new MessageData { Role = "user", Content = userMessage });

        var prompt = new ChatPrompt
        {
            Stream = true,
            Model = ModelId,
            Messages = history,
            Options = new PromptOptions
            {
                Temperature = 1.0,
                ContextLength = 8196
            }
        };

        return converstionService.SendPromptAsync(prompt);
    }

    public void AddSystemPrompt(string prompt)
    {
        MessageData systemPrompt = new MessageData
        {
            Role = "system",
            Content = prompt ?? "You are a concise and helpful assistant."
        };

        history.Add(systemPrompt);
    }
}
