using LLama;
using LLama.Common;

namespace AIServer.Llama.Models;

public class LlamaChatPrompt
{
    public SessionState SessionState { get; set; }
    public ChatHistory.Message Message { get; set; }
}