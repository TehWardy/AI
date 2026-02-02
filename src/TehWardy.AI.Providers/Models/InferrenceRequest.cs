namespace TehWardy.AI.Providers.Models;

public class InferrenceRequest
{
    public string LLMProviderName { get; set; }
    public string LLMModelName { get; set; }
    public string EmbeddingProviderName { get; set; }
    public string EmbeddingModelName { get; set; }
    public string MemoryProviderName { get; set; }

    public int ContextLength { get; set; }

    public IList<ChatMessage> Context { get; set; }
    public IList<Tool> Tools { get; set; }
}