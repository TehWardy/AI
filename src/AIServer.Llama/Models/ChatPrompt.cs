namespace AIServer.Llama.Models;

public class ChatPrompt
{
    public string Model { get; set; }
    public MessageData Message { get; set; }
    public List<MessageData> History { get; set; }
}