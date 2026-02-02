using TehWardy.AI.Models;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Fundations;

internal interface IRunbookRunnerService
{
    IAsyncEnumerable<Token> ExecuteRunbookAsync(
        RunbookRequest runbookRequest);
}