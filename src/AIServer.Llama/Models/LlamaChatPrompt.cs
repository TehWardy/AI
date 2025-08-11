using LLama.Common;

namespace AIServer.Llama.Models;

public class LlamaChatPrompt
{
    public List<ChatHistory.Message> History { get; set; }
    public ChatHistory.Message Message { get; set; }
}