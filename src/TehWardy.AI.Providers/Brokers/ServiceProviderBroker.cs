using Microsoft.Extensions.DependencyInjection;

namespace TehWardy.AI.Providers.Brokers;

internal class ServiceProviderBroker(IServiceProvider serviceProvider)
    : IServiceProviderBroker
{
    public ValueTask<T> GetNamedServiceAsync<T>(string name) =>
        ValueTask.FromResult(serviceProvider.GetRequiredKeyedService<T>(name));
}