using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.System.Models;

internal class StoredDocument
{
    public dynamic Metadata { get; set; }
    public RagDocument Document { get; set; }
};