using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Brokers;
internal interface IRunbookBroker
{
    ValueTask<Runbook> GetRunbookByNameAsync(string name);
}