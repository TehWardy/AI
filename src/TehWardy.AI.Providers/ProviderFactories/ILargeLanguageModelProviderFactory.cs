namespace TehWardy.AI.Providers.ProviderFactories;

public interface ILargeLanguageModelProviderFactory
{
    ValueTask<ILargeLanguageModelProvider> CreateLargeLanguageModelProviderAsync(string providerName);
}