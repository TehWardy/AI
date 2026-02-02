using TehWardy.AI.Providers.ProviderFactories;
using TehWardy.AI.Runbooks.Models;

namespace TehWardy.AI.Brokers;

internal class RunbookBroker(IDataCacheProviderFactory dataCacheProviderFactory)
    : IRunbookBroker
{
    public async ValueTask<Runbook> GetRunbookByNameAsync(string runbookName)
    {
        var provider = await dataCacheProviderFactory
            .CreateDataCacheProviderAsync<Runbook>("Default");

        return await provider.GetItemByKeyAsync(runbookName);
    }
}