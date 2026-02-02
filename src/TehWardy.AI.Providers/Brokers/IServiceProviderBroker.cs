namespace TehWardy.AI.Providers.Brokers;

internal interface IServiceProviderBroker
{
    ValueTask<T> GetNamedServiceAsync<T>(string name);
}