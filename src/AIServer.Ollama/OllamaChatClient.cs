using AIServer.Ollama.Models;
using AIServer.Ollama.Orchestrations;

namespace AIServer.Ollama;

public class OllamaChatClient : IOllamaChatClient
{
    private readonly IOllamaConversationOrchestrationService converstionService;

    public string ModelId { get; set; } = "gpt-oss:20b";
    public List<MessageData> history { get; set; } = [];

    public OllamaChatClient(IOllamaConversationOrchestrationService converstionService) =>
        this.converstionService = converstionService;

    public IAsyncEnumerable<ResponseToken> SendAsync(string userMessage)
    {
        if (!history.Any())
            AddSystemPrompt();

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

    void AddSystemPrompt()
    {
        MessageData systemPrompt = new MessageData
        {
            Role = "system",
            Content = "You are a concise and helpful assistant."
        };

        history.Add(systemPrompt);
    }
}
