using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Fundations;

internal interface IRunbookService
{
    ValueTask<Runbook> RetrieveRunbookByNameAsync(string name);
}