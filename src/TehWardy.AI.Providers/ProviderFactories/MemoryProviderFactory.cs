using TehWardy.AI.Providers.Foundations;

namespace TehWardy.AI.Providers.ProviderFactories;

internal class MemoryProviderFactory(IMemoryProviderService memoryProviderService) : IMemoryProviderFactory
{
    public ValueTask<IMemoryProvider> CreateMemoryProviderAsync(string providerName) =>
        memoryProviderService.GetMemoryProviderAsync(providerName);
}