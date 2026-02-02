using TehWardy.AI.Providers.ProviderFactories;

namespace TehWardy.AI.Providers.Foundations;

internal interface ILargeLanguageModelProviderService
{
    ValueTask<ILargeLanguageModelProvider> GetLargeLanguageModelProviderAsync(string providerName);
}