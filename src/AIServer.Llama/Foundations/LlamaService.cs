using AIServer.Llama.Brokers;
using AIServer.Llama.Models;
using LLama.Common;

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

    public IAsyncEnumerable<string> InitializeChatSession(string modelName)
    {
        llamaBroker.InitializeChatSession(modelName);

        var systemPrompt = new LlamaChatPrompt
        {
            Message = new ChatHistory.Message(
                authorRole: AuthorRole.System,
                content: "You are a concise assistant. keep your answers to user prompts short.")
        };

        return llamaBroker.SendPromptAsync(systemPrompt);
    }

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