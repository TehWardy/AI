namespace TehWardy.AI.Providers.Models;

public class EmbeddedSearchRequest
{
    public string ProviderName { get; set; }
    public string IndexName { get; set; }
    public string ConversationId { get; set; }
    public int TopK { get; set; }
    public float[] Embedding { get; set; }
}