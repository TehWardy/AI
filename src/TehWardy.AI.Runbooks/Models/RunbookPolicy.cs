using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Runbooks.Models;

public class RunbookPolicy
{
    public IList<Tool> AllowedTools { get; set; }
}