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
        if (llamaBroker.GetCurrentModelName() != prompt.Model)
            llamaBroker.LoadModel(prompt.Model);

        var llamaPrompt =
            MapChatPromptToLlamaChatPrompt(prompt);

        return llamaBroker.SendPromptAsync(llamaPrompt);
    }

    public void LoadModel(string modelName) =>
        llamaBroker.LoadModel(modelName);

    LlamaChatPrompt MapChatPromptToLlamaChatPrompt(ChatPrompt prompt)
    {
        return new LlamaChatPrompt
        {
            History = prompt.History
                .Select(MapMessageDataToChatHistoryMessage)
                .ToList(),

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