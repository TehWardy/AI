namespace TehWardy.AI.WebUI.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public List<ChatMessage> History { get; set; }
}
