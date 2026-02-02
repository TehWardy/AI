using TehWardy.AI.Providers.Foundations;

namespace TehWardy.AI.Providers.ProviderFactories;

internal class LargeLanguageModelProviderFactory(
    ILargeLanguageModelProviderService largeLanguageModelProviderService)
        : ILargeLanguageModelProviderFactory
{
    public ValueTask<ILargeLanguageModelProvider> CreateLargeLanguageModelProviderAsync(string providerName) =>
        largeLanguageModelProviderService.GetLargeLanguageModelProviderAsync(providerName);
}