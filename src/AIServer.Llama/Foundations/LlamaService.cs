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
            authorRole: Enum.Parse<AuthorRole>(messageData.Role),
            content: messageData.Content
        );
    }
}