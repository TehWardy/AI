namespace TehWardy.AI.Providers.ProviderFactories;

public interface IMemoryProviderFactory
{
    ValueTask<IMemoryProvider> CreateMemoryProviderAsync(string providerName);
}