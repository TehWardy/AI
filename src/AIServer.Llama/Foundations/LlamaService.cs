using AIServer.Llama.Brokers;
using AIServer.Llama.Models;
using LLama.Common;
using System.Threading.Tasks;

namespace AIServer.Llama.Foundations;

internal class LlamaService : ILlamaService
{
    private readonly ILlamaBroker llamaBroker;

    public LlamaService(ILlamaBroker llamaBroker) =>
        this.llamaBroker = llamaBroker;

    public IAsyncEnumerable<string> SendPromptAsync(ChatPrompt prompt)
    {
        var llamaPrompt =
            MapChatPromptToLlamaChatPrompt(prompt);

        return llamaBroker.SendPromptAsync(llamaPrompt);
    }

    public async ValueTask InitializeChatSession(string modelName, string systemPrompt) =>
        await llamaBroker.InitializeChatSession(modelName, systemPrompt);

    LlamaChatPrompt MapChatPromptToLlamaChatPrompt(ChatPrompt prompt)
    {
        return new LlamaChatPrompt
        {
            Message = MapMessageDataToChatHistoryMessage(prompt.Message)
        };
    }

    ChatHistory.Message MapMessageDataToChatHistoryMessage(MessageData messageData)
    {
        return new ChatHistory.Message(
            authorRole: MapAuthorFromString(messageData.Role),
            content: messageData.Content
        );
    }

    AuthorRole MapAuthorFromString(string authorRoleString)
    {
        return authorRoleString switch
        {
            "system" => AuthorRole.System,
            "user" => AuthorRole.User,
            "assistant" => AuthorRole.Assistant,
            _ => AuthorRole.Unknown
        };
    }
}
