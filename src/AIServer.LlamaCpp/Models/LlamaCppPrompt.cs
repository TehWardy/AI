namespace AIServer.LlamaCpp.Models;

public class LlamaCppPrompt
{
    public List<Message> History { get; set; }
    public Message Message { get; set; }
}