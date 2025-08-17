using AIServer.LlamaCpp.Foundations;
using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp;

public class LlamaCppChatClient : ILlamaCppChatClient
{
    private readonly ILlamaCppService chatService;

    public string ModelId { get; set; } = "gpt-oss:20b";
    public List<Message> History { get; set; } = [];

    public LlamaCppChatClient(ILlamaCppService chatService) =>
        this.chatService = chatService;

    public async IAsyncEnumerable<string> SendAsync(string userMessage)
    {
        if (!History.Any())
            AddSystemPrompt();

        var prompt = new LlamaCppPrompt
        {

            History = History,
            Message = new Message
            {
                Role = "user",
                Content = userMessage
            }
        };

        History.Add(prompt.Message);

        var responseMessage = new Message
        {
            Role = "user",
            Content = string.Empty
        };

        await foreach (var token in chatService.SendPromptAsync(prompt))
        {
            responseMessage.Content += token;
            yield return token;
        }

        History.Add(responseMessage);
    }

    void AddSystemPrompt()
    {
        Message systemPrompt = new Message
        {
            Role = "system",
            Content = "You are a concise and helpful assistant."
        };

        History.Add(systemPrompt);
    }
}