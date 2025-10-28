using AIServer.LlamaCpp.Models;
using AIServer.LlamaCpp.Processings;

namespace AIServer.LlamaCpp;

public class LlamaCppChatClient : ILlamaCppChatClient
{
    private readonly ILlamaCppProcessingService chatService;

    public string ModelId { get; set; } = "gpt-oss:20b";
    public List<Message> History { get; set; } = [];

    public LlamaCppChatClient(ILlamaCppProcessingService chatService) =>
        this.chatService = chatService;

    public async IAsyncEnumerable<ResponseToken> SendAsync(string userMessage)
    {
        if (!History.Any())
            AddSystemPrompt();

        var userPrompt = new Message
        {
            Role = "user",
            Content = userMessage
        };

        History.Add(userPrompt);

        var responseMessage = new Message
        {
            Role = "assistant",
            Content = string.Empty
        };

        await foreach (var token in chatService.SendPromptAsync(History))
        {
            responseMessage.Content += token.Message.Content;
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