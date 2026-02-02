namespace TehWardy.AI.WebUI.Models;

public class ChatMessage
{
    public string Role { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Thought { get; set; } = string.Empty;
}