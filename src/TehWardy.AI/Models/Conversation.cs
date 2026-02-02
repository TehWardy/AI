using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public List<ChatMessage> History { get; set; }
}