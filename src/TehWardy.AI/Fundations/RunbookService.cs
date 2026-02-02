using TehWardy.AI.Brokers;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Fundations;

internal class RunbookService(IRunbookBroker runbookBroker) : IRunbookService
{
    public ValueTask<Runbook> RetrieveRunbookByNameAsync(string name) =>
        runbookBroker.GetRunbookByNameAsync(name);
}