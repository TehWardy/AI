using System.Text.Json;
using TehWardy.AI.Providers.Brokers;

namespace TehWardy.AI.Providers.AI.Foundations;

internal class RoutingService(IRoutingBroker routingBroker)
    : IRoutingService
{
    public async ValueTask<string> RetrieveAllRoutingInformationAsync<T>()
    {
        object routingInformation = await routingBroker
            .RetrieveAllRoutingInformationAsync<T>();

        return JsonSerializer.Serialize(routingInformation);
    }
}