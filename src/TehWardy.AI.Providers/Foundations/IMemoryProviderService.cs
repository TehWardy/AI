using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal interface IMemoryProviderService
{
    ValueTask<IMemoryProvider> GetMemoryProviderAsync(string providerName);
}