namespace TehWardy.AI.Providers.Models;

public class RagDocument
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public string Content { get; set; }
    public float[] ContentVector { get; set; }
}